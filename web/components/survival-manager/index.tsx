/**
 * SURVIVAL MODE MANAGER
 * Generates new levels for each round and handles the game mode
 * 
 */

"use client";

import useLevelGenerator from "@/hooks/use-level-generator";
import { useSurvivalModeStore } from "@/store/use-survival-mode-store";
import { FC, useCallback, useEffect, useState } from "react";
import { ReactUnityEventParameter } from "react-unity-webgl/distribution/types/react-unity-event-parameters";


interface SurvivalManagerProps {
	addEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	removeEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	sendMessage: (gameObjectName: string, methodName: string, parameter?: ReactUnityEventParameter) => void
}


const SurvivalManager: FC<SurvivalManagerProps> = ({ addEventListener, removeEventListener, sendMessage }) => {


	// Global Store
	const { gridData, setGridData } = useSurvivalModeStore();


	// Local State
	const [startNewLevel, setStartNewLevel] = useState(false);


	// Hooks
	const { generateLevel } = useLevelGenerator();


	// Listen for the start new level event
	const receivedStartNewLevelEvent = useCallback(() => {
		console.log("🔃 Requesting survival level...");
		setStartNewLevel(true);
	}, []);

	useEffect(() => {
		addEventListener("RequestSurvivalLevel", receivedStartNewLevelEvent);
		return () => removeEventListener("RequestSurvivalLevel", receivedStartNewLevelEvent);
	}, [addEventListener, removeEventListener, receivedStartNewLevelEvent]);


	// Generate a new AI level
	const handleGenerateNewLevel = async () => {
		const generatedLevel = await generateLevel();
		if (generatedLevel) setGridData(generatedLevel);
	}


	// Start a new round
	useEffect(() => {
		if (startNewLevel) {
			handleStartNewRound();
			setStartNewLevel(false);
			handleGenerateNewLevel();
		}
	}, [startNewLevel]);

	const handleStartNewRound = () => {
		sendMessage("GameManager", "StartNewRound", `{grid: ${gridData}}`);
	}


	return null;
}

export default SurvivalManager;