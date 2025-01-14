/**
 * LEVEL GENERATOR
 * Uses AI to generate new levels and player can play them
 * 
 */

"use client";

import { FC, useEffect, useState } from "react";
import { useApplicationStore } from "@/store/use-application-store";
import Dialog from "../ui/dialog";
import { Button } from "../ui/button";
import MorphingText from "../ui/morphing-text";
import LevelLoader from "./level-loader";
import LevelPreview from "./level-preview";
import { convertToNumberArray } from "@/lib/convert-to-number-array";
import { ReactUnityEventParameter } from "react-unity-webgl/distribution/types/react-unity-event-parameters";
import useLevelGenerator from "@/hooks/use-level-generator";


// Messages displayed while the AI is generating a level
const startScreenTexts = [
	"Welcome!",
	"The AI overlord shapes the world.",
	"Are you prepared?",
	"It builds, you survive.",
	"Adapt or be erased.",
	"Each move shapes your fate.",
	"It evolves, do you?",
	"The world bends to its will.",
	"Face what it creates.",
	"You’re a pawn in its game.",
	"Survive its design."
];


const creationTexts = [
	"Shaping your challenge...",
	"Designing the impossible...",
	"Building the unknown...",
	"It’s coming together...",
	"Crafting your path...",
	"Reality is forming...",
	"Piecing it all together...",
	"Constructing your doom...",
	"The world is taking shape...",
	"Assembling the chaos..."
];


enum GenerationStep {
	StartScreen,
	Generating,
	Generated,
	Error
}


// Max number of generations
const MAX_GENERATIONS = 10;
const STORAGE_KEY = 'levelGenerations';


interface LevelGeneratorProps {
	sendMessage: (gameObjectName: string, methodName: string, parameter?: ReactUnityEventParameter) => void;
}


const LevelGenerator: FC<LevelGeneratorProps> = ({ sendMessage }) => {


	// Global Stores
	const { setMainMenuActive, setGameModeActive, isLevelGeneratorActive, setIsLevelGeneratorActive, setGeneratedLevelData, setCustomGameLaunchedFrom } = useApplicationStore();


	// Local States
	const [generationStep, setGenerationStep] = useState(GenerationStep.StartScreen);
	const [generationsUsed, setGenerationsUsed] = useState(0);
	const [gridData, setGridData] = useState<string | null>(null);


	// Hooks
	const { generateLevel } = useLevelGenerator();


	// Initialize generations count from localStorage
	useEffect(() => {
		const storedGenerations = localStorage.getItem(STORAGE_KEY);
		const usedGenerations = storedGenerations ? parseInt(storedGenerations) : 0;
		setGenerationsUsed(usedGenerations);
	}, []);


	// Start generating new level
	const handleGenerateLevel = async () => {

		if (generationsUsed >= MAX_GENERATIONS) return alert("You have used all your credits. Play more in the Level Browser.");
		setGenerationStep(GenerationStep.Generating);

		// Update localStorage before generating
		const newGenerationCount = generationsUsed + 1;
		localStorage.setItem(STORAGE_KEY, newGenerationCount.toString());
		setGenerationsUsed(newGenerationCount);

		// Generate level using AI
		const generatedLevel = await generateLevel();

		if (generatedLevel) {
			setGridData(generatedLevel);
			setGenerationStep(GenerationStep.Generated);
		}
	}


	// Play the newly generated level
	const handlePlayLevel = () => {
		if (!gridData) return;

		// Used for the level uploader
		const generatedLevel = convertToNumberArray(gridData);
		setGeneratedLevelData(generatedLevel);

		setIsLevelGeneratorActive(false);
		setMainMenuActive(false);
		setGameModeActive('custom');
		setCustomGameLaunchedFrom("ai-generator");

		sendMessage("MainMenuManager", "StartAILevelMode", `{grid: ${gridData}}`);
		setGenerationStep(GenerationStep.StartScreen);
	}


	// Get the text to display for the generations used
	const getGenerationsText = () => {
		return `${generationsUsed} / ${MAX_GENERATIONS} AI levels generated. ${generationsUsed >= MAX_GENERATIONS ? 'Play more in the Level Browser.' : ''}`;
	}


	return (
		<Dialog
			open={isLevelGeneratorActive}
			onOpenChange={(generationStep === GenerationStep.Generating || generationStep === GenerationStep.Generated) ? () => { } : setIsLevelGeneratorActive}
			className="max-w-5xl min-h-[calc(100vh_-_10rem)]"
		>
			<div className="relative z-10 h-full flex flex-col justify-center items-center space-y-10">

				{generationStep === GenerationStep.StartScreen &&
					<>
						<div className="w-full text-center space-y-1">
							<MorphingText texts={startScreenTexts} className="font-orbitron" />
						</div>

						<div className="flex items-center gap-2">
							<Button variant={"outline"} size={"lg"} onClick={() => setIsLevelGeneratorActive(false)}>
								Back
							</Button>

							<Button variant={"orange"} size={"lg"} onClick={handleGenerateLevel}>
								Start Challenge
							</Button>
						</div>
					</>
				}

				{generationStep === GenerationStep.Generating &&
					<>
						<MorphingText texts={creationTexts} className="font-orbitron" />
						<LevelLoader />
					</>
				}

				{(generationStep === GenerationStep.Generated && gridData) &&
					<>
						<h1 className="font-orbitron text-4xl">Your challenge is ready!</h1>
						<LevelPreview animate={true} initialGrid={convertToNumberArray(gridData)} />
						<Button variant={"orange"} size={"lg"} onClick={handlePlayLevel}>
							Play Challenge
						</Button>
					</>
				}

				<div className="absolute bottom-0 left-0 right-0 text-center text-muted-foreground text-sm">
					{getGenerationsText()}
				</div>
			</div>
		</Dialog>
	)
}

export default LevelGenerator;