import { create } from "zustand";
import { Schema } from "@/amplify/data/resource";
export type AiLevelWithCoverImage = Schema["AiLevel"]["type"] & {
	coverImage?: string;
};
interface LevelsStore {
	levels: Array<AiLevelWithCoverImage> | undefined;
	setLevels: (levels: Array<Schema["AiLevel"]["type"]>) => void;
	getRandomLevelsGrid: (amount: number) => string[] | undefined;
}
export const useLevelsStore = create<LevelsStore>((set) => ({
	levels: undefined,
	setLevels: (levels) => set({ levels }),

	getRandomLevelsGrid: (amount: number): string[] | undefined => {
		const { levels } = useLevelsStore.getState();

		if (!levels || levels.length === 0) {
			return undefined;
		}

		if (amount > levels.length) {
			console.log("Not enough levels in the library to generate requested amount of levels.");
			return undefined;
		}

		// Create a copy of levels array to avoid mutating original
		const levelsCopy = [...levels];
		const selectedLevels: string[] = [];

		// Select random levels
		for (let i = 0; i < amount; i++) {
			const randomIndex = Math.floor(Math.random() * levelsCopy.length);
			const level = levelsCopy.splice(randomIndex, 1)[0];
			if (level.grid) {
				selectedLevels.push(level.grid);
			}
		}

		return selectedLevels;
	},
}));