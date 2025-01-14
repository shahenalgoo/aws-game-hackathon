import { create } from "zustand";

interface SurvivalModeStore {
	gridData: string;
	setGridData: (newValue: string) => void;
	submitDialogActive: boolean;
	setSubmitDialogActive: (submitDialogActive: boolean) => void;
}

export const useSurvivalModeStore = create<SurvivalModeStore>((set) => ({
	gridData: "[[0,0,0,0,2,1,4,1,0,0,0,0,2,1,0,0,0,2,0,0],[0,2,3,1,0,0,0,5,1,2,0,3,1,4,1,2,0,1,0,0],[0,8,7,2,0,2,0,0,0,1,0,1,0,0,0,1,4,2,0,0],[0,1,0,1,4,1,2,0,0,5,1,2,0,2,3,1,0,6,1,0],[1,1,0,2,0,0,6,1,2,1,0,0,0,1,0,4,1,0,2,1],[0,2,0,1,3,0,0,0,0,4,1,3,0,5,0,0,2,0,0,0],[0,1,4,2,1,2,0,2,0,0,0,1,2,1,6,1,0,0,0,0],[0,3,0,0,0,5,1,1,2,0,0,0,0,0,2,5,1,1,0,0],[0,2,0,0,0,1,0,0,6,1,2,0,0,0,1,0,0,2,0,0]]",
	setGridData: (newValue) => set({ gridData: newValue }),
	submitDialogActive: false,
	setSubmitDialogActive: (submitDialogActive) => set({ submitDialogActive }),
}));