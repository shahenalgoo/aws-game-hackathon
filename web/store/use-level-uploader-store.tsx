import { create } from "zustand";

interface LevelUploaderStore {
	hasUploadedLevel: boolean;
	setHasUploadedLevel: (hasUploadedLevel: boolean) => void;
}

export const useLevelUploaderStore = create<LevelUploaderStore>((set) => ({
	hasUploadedLevel: false,
	setHasUploadedLevel: (hasUploadedLevel) => set({ hasUploadedLevel }),
}));