/**
 * OVERLORD DIALOG
 * Generates new voicelines and plays them when the Overlord dialog is triggered.
 * 
 */

"use client";

import { FC, useCallback, useEffect, useRef, useState } from "react";
import Image from "next/image";
import { useApplicationStore } from "@/store/use-application-store";
import useOverlordStore, { VoicelineType } from "@/store/use-overlord-store";
import useGenerateAudio from "../hooks/use-generate-audio";
import { ReactUnityEventParameter } from "react-unity-webgl/distribution/types/react-unity-event-parameters";
import { BorderBeam } from "./ui/border-beam";


interface OverlordDialogProps {
	addEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	removeEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	sendMessage: (gameObjectName: string, methodName: string, parameter?: ReactUnityEventParameter) => void;
}

const OverlordDialog: FC<OverlordDialogProps> = ({ addEventListener, removeEventListener, sendMessage }) => {


	// Global Store
	const { gameModeActive } = useApplicationStore();
	const { generateVoicelines, pickVoiceline, audio, audioStopped, setAudioStopped, stopOverlordAudio } = useOverlordStore();


	// Local States
	const [isTriggered, setIsTriggered] = useState<boolean>(false);
	const [showDialog, setShowDialog] = useState<boolean>(false);
	const [voicelineType, setVoicelineType] = useState<VoicelineType>("spawn");
	const [voicelineToPlay, setVoicelineToPlay] = useState<string | null>(null);


	// Hooks
	const { generateAudio } = useGenerateAudio();


	// Generate AI Overlord's voicelines
	// Prefetch voicelines to add to the voicelines array
	const hasRun = useRef(false);

	useEffect(() => {
		if (hasRun.current) return;
		hasRun.current = true;
		generateVoicelines();
	}, []);


	// Overlord Voice line Events
	// Received from the game
	const receivedVoicelineEvent = useCallback((voicelineType: any) => {
		setVoicelineType(voicelineType);
		setIsTriggered(true);
	}, []);

	useEffect(() => {
		addEventListener("PlayVoiceline", receivedVoicelineEvent);
		return () => removeEventListener("PlayVoiceline", receivedVoicelineEvent);
	}, [addEventListener, removeEventListener, receivedVoicelineEvent]);


	// VOICE EVENT LINE IS TRIGGERED
	// If PlayVoiceline event is triggered start picking and generating voice line
	useEffect(() => {
		if (!isTriggered) return;
		handleVoicelines();
	}, [isTriggered]);


	// Pick & generate Overlord's voice 
	const handleVoicelines = () => {
		const selectedVoiceline = pickVoiceline(voicelineType);
		if (!selectedVoiceline) return;
		setVoicelineToPlay(selectedVoiceline);
		generateAudio(selectedVoiceline, handleCanPlaythrough, handleAudioEnd);
	}


	// When audio is ready to play
	const handleCanPlaythrough = () => {
		sendMessage("AudioManager", "SetVolume", 0.1);
		setShowDialog(true);
	}


	// When audio ends
	const handleAudioEnd = () => {
		stopOverlordAudio();
		setIsTriggered(false);
		setShowDialog(false);
		sendMessage("AudioManager", "SetVolume", 1);
	};


	// Stop everything if audio is stopped
	useEffect(() => {
		if (audioStopped) {
			setIsTriggered(false);
			setShowDialog(false);
			setAudioStopped(false);
		}
	}, [audioStopped]);


	// If game mode is not active, never show the AI Overlord dialog
	if (gameModeActive === "none") return null;


	return showDialog ? (
		<div className={`
			absolute z-[100] left-10 bottom-72 max-w-96 flex items-center gap-4 cursor-default select-none `}>

			<figure className="shrink-0 overflow-hidden w-20 h-20 rounded-xl bg-white/50 border-2 border-red-600 shadow-md">
				<Image src="/overlord.jpg" width={200} height={200} priority quality={100} alt="AI Overlord" className="w-auto h-auto" onDragStart={(e) => e.preventDefault()} />
			</figure>

			<div className="w-full backdrop-blur-sm bg-black/60 rounded-xl px-4 py-2 font-orbitron">
				<BorderBeam size={100} colorFrom="#dc2626" colorTo="#f87171" borderWidth={2} duration={5} />
				{voicelineToPlay}
			</div>

		</div>
	) : null;
}

export default OverlordDialog;