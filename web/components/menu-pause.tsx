/**
 * PAUSE MENU
 * Displays a pause menu with options to resume, restart, or exit to main menu
 * 
 */

"use client";

import { useApplicationStore } from "@/store/use-application-store";
import { FC, useCallback, useEffect, useState } from "react";
import { ReactUnityEventParameter } from "react-unity-webgl/distribution/types/react-unity-event-parameters";
import useFullscreen from "@/hooks/use-fullscreen";
import useOverlordStore from "@/store/use-overlord-store";
import Dialog from "./ui/dialog";
import { Button } from "./ui/button";
import { Switch } from "./ui/switch";
import { Label } from "./ui/label";
import { useLevelUploaderStore } from "@/store/use-level-uploader-store";


interface MenuPause {
	addEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	removeEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	sendMessage: (gameObjectName: string, methodName: string, parameter?: ReactUnityEventParameter) => void;
}


enum ConfirmationAction {
	None = "none",
	Restart = "restart",
	Exit = "exit"
}


const MenuPause: FC<MenuPause> = ({
	addEventListener,
	removeEventListener,
	sendMessage
}) => {


	// Global Store
	const { menuPauseActive, setMenuPauseActive, gameModeActive } = useApplicationStore();
	const { setHasUploadedLevel } = useLevelUploaderStore();
	const { pauseAudio, resumeAudio, stopOverlordAudio } = useOverlordStore();


	// Local States
	const [sfx, setSfx] = useState(true);
	const [music, setMusic] = useState(true);
	const [confirmationActive, setConfirmationActive] = useState(false);
	const [confirmationAction, setConfirmationAction] = useState<ConfirmationAction>(ConfirmationAction.None);


	// Hooks
	const { isFullscreen, toggleFullscreen } = useFullscreen();


	// Receive event to pause menu
	const receivedActivatePauseMenu = useCallback((sfxMute: any, musicMute: any) => {
		pauseAudio()
		setMenuPauseActive(true);
		sfxMute === 0 ? setSfx(true) : setSfx(false);
		musicMute === 0 ? setMusic(true) : setMusic(false);
	}, []);

	useEffect(() => {
		addEventListener("ActivatePauseMenu", receivedActivatePauseMenu);
		return () => removeEventListener("ActivatePauseMenu", receivedActivatePauseMenu);
	}, [addEventListener, removeEventListener, receivedActivatePauseMenu]);


	// Receive event to unpause menu
	const receivedDeactivatePauseMenu = useCallback(() => {
		resumeAudio();
		setMenuPauseActive(false);
	}, []);

	useEffect(() => {
		addEventListener("DeactivatePauseMenu", receivedDeactivatePauseMenu);
		return () => removeEventListener("DeactivatePauseMenu", receivedDeactivatePauseMenu);
	}, [addEventListener, removeEventListener, receivedDeactivatePauseMenu]);


	// Close pause menu
	function handleClosePauseMenu() {
		setMenuPauseActive(false);
		sendMessage("UICanvas", "TogglePauseGame")
	}


	// Mute SFX
	function handleMuteSFX() {
		setSfx(!sfx);
		sendMessage("UICanvas", "ToggleSFXFromReact");
	}


	// Mute Music
	function handleMuteMusic() {
		setMusic(!music);
		sendMessage("UICanvas", "ToggleMusicFromReact");
	}


	// CONFIRMATION
	// Restart or Exit confirmation handling
	function handleGameAction(action: ConfirmationAction, isConfirmation: boolean = false) {
		if (!isConfirmation) {
			setConfirmationActive(true);
			setConfirmationAction(action);
			return;
		}

		if (action === ConfirmationAction.None) {
			setConfirmationActive(false);
			setConfirmationAction(ConfirmationAction.None);
			return;
		}

		stopOverlordAudio();
		sendMessage("AudioManager", "SetVolume", 1);

		const actions = {
			[ConfirmationAction.Restart]: () => sendMessage("UICanvas", "RestartLevel"),
			[ConfirmationAction.Exit]: () => {
				sendMessage("UICanvas", "ExitToMainMenu");
				setHasUploadedLevel(false);
			}
		};

		setMenuPauseActive(false);
		actions[action]?.();

		setTimeout(() => {
			setConfirmationActive(false);
			setConfirmationAction(ConfirmationAction.None);
		}, 1000);
	}


	return (
		<Dialog
			open={menuPauseActive}
			onOpenChange={() => { }}
			className="overflow-hidden max-w-xs"
		>
			<div className="relative z-10 flex flex-col items-center justify-center">

				<div className="mb-6 text-center">
					{!confirmationActive
						? <h2 className="text-3xl font-orbitron tracking-wide">Paused</h2>
						: <h2 className="text-2xl font-orbitron tracking-wide">Are you sure?</h2>
					}
					<span className="text-sm text-muted-foreground">
						{!confirmationActive
							? <>{gameModeActive === "normal" ? "Campaign Mode" : gameModeActive === "bossFight" ? "Boss Fight" : gameModeActive === "tutorial" ? "Tutorial" : gameModeActive === "survival" ? "Survival" : "AI Generated Level"}</>
							: <>{confirmationAction === "restart" ? "Restart Game" : "Exit to main menu"}</>
						}
					</span>
				</div>

				{!confirmationActive &&
					<>
						<div className="space-y-3 outline-none">
							<Button onClick={handleClosePauseMenu} variant={"secondary"} className="w-full shadow-none" tabIndex={-1}>Resume</Button>
							{gameModeActive !== "survival" && <Button onClick={() => handleGameAction(ConfirmationAction.Restart)} variant={"secondary"} className="w-full" tabIndex={-1}>
								{gameModeActive === "normal" ? "Restart Checkpoint" : "Restart"}
							</Button>}
							<Button onClick={() => handleGameAction(ConfirmationAction.Exit)} variant={"secondary"} className="w-full" tabIndex={-1}>Exit to Main Menu</Button>
						</div>

						<div className="my-4 w-10 h-[1px] mx-auto bg-white/10" />

						<Button onClick={toggleFullscreen} variant={"outline"} size={"sm"} tabIndex={-1}>{isFullscreen ? "Exit Fullscreen" : "Enter Fullscreen"}</Button>

						<div className="my-4 w-10 h-[1px] mx-auto bg-white/10" />

						<div className="relative flex items-center border rounded-2xl px-2">
							<div className="flex items-center">
								<Switch id="sfx" checked={sfx} onCheckedChange={handleMuteSFX} tabIndex={-1} />
								<Label htmlFor="sfx" className="cursor-pointer pl-2 py-2">SFX</Label>
							</div>

							<div className="mx-6 w-[1px] h-4 bg-white/10" />

							<div className="flex items-center">
								<Switch id="music" checked={music} onCheckedChange={handleMuteMusic} tabIndex={-1} />
								<Label htmlFor="music" className="cursor-pointer pl-2 py-2">Music</Label>
							</div>
						</div>

					</>
				}

				{confirmationActive &&
					<div className="w-full flex items-center space-x-2">
						<Button onClick={() => handleGameAction(ConfirmationAction.None, true)} variant={"outline"} className="!flex-1 shadow-none" tabIndex={-1}>No</Button>
						<Button
							onClick={() => {
								if (confirmationAction === ConfirmationAction.Restart) handleGameAction(ConfirmationAction.Restart, true);
								else if (confirmationAction === ConfirmationAction.Exit) handleGameAction(ConfirmationAction.Exit, true);
							}}
							variant={"orange"}
							className="!flex-1 shadow-none"
							tabIndex={-1}
						>
							Yes
						</Button>
					</div>
				}

			</div>
		</Dialog>
	);
}

export default MenuPause;