import { create } from 'zustand';

export type GameMode = 'none' | 'normal' | 'custom' | 'bossFight' | 'tutorial' | 'survival';
export type CustomGameLaunchedFrom = 'none' | 'ai-generator' | 'level-browser';

interface ApplicationStore {

	mainMenuActive: boolean;
	setMainMenuActive: (mainMenuActive: boolean) => void;


	gameModeActive: GameMode;
	setGameModeActive: (gameModeActive: GameMode) => void;


	customGameLaunchedFrom: CustomGameLaunchedFrom;
	setCustomGameLaunchedFrom: (customGameLaunchedFrom: CustomGameLaunchedFrom) => void;


	isLevelGeneratorActive: boolean;
	setIsLevelGeneratorActive: (isLevelGeneratorActive: boolean) => void;


	generatedLevelData: number[][] | undefined;
	setGeneratedLevelData: (generatedLevelData: number[][] | undefined) => void;


	levelBrowserActive: boolean;
	setLevelBrowserActive: (levelBrowserActive: boolean) => void;


	submitDialogActive: boolean;
	setSubmitDialogActive: (submitDialogActive: boolean) => void;

	leaderboardDialogActive: boolean;
	setLeaderboardDialogActive: (leaderboardDialogActive: boolean) => void;



	menuPauseActive: boolean;
	setMenuPauseActive: (menuPauseActive: boolean) => void;


	menuDeathActive: boolean;
	setMenuDeathActive: (menuDeathActive: boolean) => void;


	creditsDialogActive: boolean;
	setCreditsDialogActive: (creditsDialogActive: boolean) => void;

}

export const useApplicationStore = create<ApplicationStore>((set) => ({

	mainMenuActive: false,
	setMainMenuActive: (mainMenuActive) => set({ mainMenuActive }),


	gameModeActive: 'none',
	setGameModeActive: (gameModeActive) => set({ gameModeActive }),


	customGameLaunchedFrom: 'none',
	setCustomGameLaunchedFrom: (customGameLaunchedFrom) => set({ customGameLaunchedFrom }),


	isLevelGeneratorActive: false,
	setIsLevelGeneratorActive: (isLevelGeneratorActive) => set({ isLevelGeneratorActive }),


	generatedLevelData: undefined,
	setGeneratedLevelData: (generatedLevelData) => set({ generatedLevelData }),


	levelBrowserActive: false,
	setLevelBrowserActive: (levelBrowserActive) => set({ levelBrowserActive }),


	submitDialogActive: false,
	setSubmitDialogActive: (submitDialogActive) => set({ submitDialogActive }),


	leaderboardDialogActive: false,
	setLeaderboardDialogActive: (leaderboardDialogActive) => set({ leaderboardDialogActive }),


	menuPauseActive: false,
	setMenuPauseActive: (menuPauseActive) => set({ menuPauseActive }),

	menuDeathActive: false,
	setMenuDeathActive: (menuDeathActive) => set({ menuDeathActive }),

	creditsDialogActive: false,
	setCreditsDialogActive: (creditsDialogActive) => set({ creditsDialogActive }),

}));