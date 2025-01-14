/**
 * MAIN MENU
 * The main menu is an overlay that is only displayed when the "ActivateMainMenu" event is sent by the game.
 * Includes: a list of buttons to start different game modes or other features.
 * 
 */

"use client";

import { FC, useCallback, useEffect, useState } from "react";
import Image from "next/image";
import { useApplicationStore } from "@/store/use-application-store";
import { ReactUnityEventParameter } from "react-unity-webgl/distribution/types/react-unity-event-parameters";
import MainMenuBtn from "./main-menu-btn";
import { MainMenuSectionPrimary, MainMenuSectionSecondary, MainMenuSection } from "./main-menu-section";
import { Button } from "../ui/button";
import { signOut } from "aws-amplify/auth";
import { useSurvivalModeStore } from "@/store/use-survival-mode-store";
import useLevelGenerator from "@/hooks/use-level-generator";
import { useLevelsStore } from "@/store/use-level-store";


interface MainMenuProps {
	addEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	removeEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	sendMessage: (gameObjectName: string, methodName: string, parameter?: ReactUnityEventParameter) => void
}


// Helper function to check tutorial completion
const isTutorialCompleted = (): boolean => {
	return localStorage.getItem('tutorialCompleted') === "true";
};


const MainMenu: FC<MainMenuProps> = ({ addEventListener, removeEventListener, sendMessage }) => {


	// Global Store
	const { mainMenuActive, setMainMenuActive, setGameModeActive, setIsLevelGeneratorActive, setLevelBrowserActive, setLeaderboardDialogActive, setCreditsDialogActive } = useApplicationStore();
	const { setGridData } = useSurvivalModeStore();
	const { levels, getRandomLevelsGrid } = useLevelsStore();


	// Local States
	const [showTutorial, setShowTutorial] = useState(false);


	// Hooks
	const { generateLevel } = useLevelGenerator();


	// Force first time players to play the tutorial
	useEffect(() => {
		setShowTutorial(!isTutorialCompleted());
	}, []);


	// Listens for the "ActivateMainMenu" event and sets the main menu active
	const receivedMainMenuEvent = useCallback(() => {
		setMainMenuActive(true);
		setGameModeActive('none');
		setShowTutorial(!isTutorialCompleted());
	}, []);

	useEffect(() => {
		addEventListener("ActivateMainMenu", receivedMainMenuEvent);
		return () => removeEventListener("ActivateMainMenu", receivedMainMenuEvent);
	}, [addEventListener, removeEventListener, receivedMainMenuEvent]);


	// Tutorial mode
	function handleStartTutorial() {
		setMainMenuActive(false);
		setGameModeActive('tutorial');
		sendMessage("MainMenuManager", "StartTutorial");
		localStorage.setItem('tutorialCompleted', 'true');
	}


	// Campaign mode
	// Selects 3 random levels from the level browser
	// Has a fallback to default levels if the browser is empty
	function handleStartCampaignMode() {
		setMainMenuActive(false);
		setGameModeActive('normal');

		const randomLevels = getRandomLevelsGrid(3);

		// If no levels are found, use fallback levels
		if (randomLevels === undefined) {
			sendMessage("MainMenuManager", "StartNormalMode");
			return;
		}

		for (let i = 0; i < randomLevels.length; i++) {
			randomLevels[i] = `{grid: ${randomLevels[i]}}`;
		}

		const playlist = { playlist: randomLevels };
		sendMessage("MainMenuManager", "StartCampaignMode", JSON.stringify(playlist));
	}


	// Boss fight mode
	const handleStartBossFightMode = () => {
		setMainMenuActive(false);
		setGameModeActive('bossFight');
		sendMessage("MainMenuManager", "StartBossFight");
	}


	// Survival mode
	// Uses a random level from the level browser
	// Has a fallback to default levels if the browser is empty
	const handleStartSurvivalMode = async () => {
		setMainMenuActive(false);
		setGameModeActive('survival');

		if (levels && levels.length > 0) {
			// Get random level from existing level library
			const fallbackLevel = "[[0,0,0,0,2,1,4,1,0,0,0,0,2,1,0,0,0,2,0,0],[0,2,3,1,0,0,0,5,1,2,0,3,1,4,1,2,0,1,0,0],[0,8,7,2,0,2,0,0,0,1,0,1,0,0,0,1,4,2,0,0],[0,1,0,1,4,1,2,0,0,5,1,2,0,2,3,1,0,6,1,0],[1,1,0,2,0,0,6,1,2,1,0,0,0,1,0,4,1,0,2,1],[0,2,0,1,3,0,0,0,0,4,1,3,0,5,0,0,2,0,0,0],[0,1,4,2,1,2,0,2,0,0,0,1,2,1,6,1,0,0,0,0],[0,3,0,0,0,5,1,1,2,0,0,0,0,0,2,5,1,1,0,0],[0,2,0,0,0,1,0,0,6,1,2,0,0,0,1,0,0,2,0,0]]";
			const randomLevel = getRandomLevelsGrid(1);
			sendMessage("MainMenuManager", "StartSurvivalMode", `{grid: ${randomLevel == undefined ? fallbackLevel : randomLevel}}`);

			// PreGenerate a new AI level
			const generatedLevel = await generateLevel();
			if (generatedLevel) setGridData(generatedLevel);
		}
	}


	// Sign out from the game
	const handleSignOut = async () => {
		await signOut();
		window.location.reload();
	}


	// If the main menu is not active, don't render anything
	if (!mainMenuActive) return null;


	return (
		<div className="fixed top-0 left-0 z-40 w-2/3 h-full flex justify-center items-center">

			<div className="relative z-30 flex flex-col justify-center items-center space-y-8">
				<Image src="/logo.png" alt="logo" width={720} height={215} className="w-auto h-auto max-w-xl relative z-50" priority />

				{showTutorial &&
					<div className="p-6 bg-background/90 rounded-xl text-center">
						<p className="mb-4 text-muted-foreground">Since this is your first time playing, we recommend <br /> you start with the tutorial.</p>
						<Button variant={"default"} onClick={handleStartTutorial} tabIndex={-1}>Start Tutorial</Button>
					</div>
				}

				{!showTutorial &&
					<>
						<MainMenuSection title="Normal Mode">
							<MainMenuSectionPrimary>
								<MainMenuBtn onClick={handleStartCampaignMode} title="Play Campaign" />
							</MainMenuSectionPrimary>

							<MainMenuSectionSecondary>
								<MainMenuBtn onClick={handleStartTutorial} title="Tutorial" />
								<MainMenuBtn onClick={handleStartBossFightMode} title="Boss Fight" />
							</MainMenuSectionSecondary>
						</MainMenuSection>


						<MainMenuSection title="AI Generated Levels">
							<MainMenuSectionPrimary>
								<MainMenuBtn onClick={handleStartSurvivalMode} title="Survival Mode" />
							</MainMenuSectionPrimary>
							<MainMenuSectionSecondary>
								<MainMenuBtn onClick={() => setIsLevelGeneratorActive(true)} title="Generate" />
								<MainMenuBtn onClick={() => setLevelBrowserActive(true)} title="Browse" />

							</MainMenuSectionSecondary>
						</MainMenuSection>


						<MainMenuSection title="Other">
							<MainMenuSectionPrimary>
								<MainMenuBtn onClick={() => setLeaderboardDialogActive(true)} title="Leaderboards" />
							</MainMenuSectionPrimary>

							<MainMenuSectionSecondary>
								<MainMenuBtn onClick={() => setCreditsDialogActive(true)} title="Credits" />
								<MainMenuBtn onClick={handleSignOut} title="Sign Out" />
							</MainMenuSectionSecondary>
						</MainMenuSection>
					</>
				}
			</div>
		</div>
	)
}

export default MainMenu;