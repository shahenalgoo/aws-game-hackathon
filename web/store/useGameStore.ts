import { create } from "zustand";

interface GameStore {
	isUnityLoaded: boolean;
	setIsUnityLoaded: (isUnityLoaded: boolean) => void;

	isMainMenuActive: boolean;
	setIsMainMenuActive: (isMainMenuActive: boolean) => void;

	isLevelSelectorActive: boolean;
	setIsLevelSelectorActive: (isLevelSelectorActive: boolean) => void;
}

const useGameStore = create<GameStore>((set) => ({
	isUnityLoaded: false,
	setIsUnityLoaded: (isUnityLoaded) => set({ isUnityLoaded }),

	isMainMenuActive: false,
	setIsMainMenuActive: (isMainMenuActive) => set({ isMainMenuActive }),

	isLevelSelectorActive: false,
	setIsLevelSelectorActive: (isLevelSelectorActive) => set({ isLevelSelectorActive }),
}));

export default useGameStore;