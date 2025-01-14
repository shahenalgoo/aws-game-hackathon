import { create } from "zustand";
import { type RefObject } from 'react';

type DivRef = RefObject<HTMLDivElement | null>

interface RefStore {
	containerRef: DivRef | null;
	setContainerRef: (ref: DivRef) => void;
}

export const useRefStore = create<RefStore>((set) => ({
	containerRef: null,
	setContainerRef: (ref) => set({ containerRef: ref })
}));