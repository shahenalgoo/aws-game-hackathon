"use client";

import { FC } from "react";
import useGameStore from "@/store/useGameStore";
import GameDialog from "./dialog";
import useRefStore from "@/store/useRefStore";

interface LevelSelectorProps {

}

const LevelSelector: FC<LevelSelectorProps> = () => {


	const { isLevelSelectorActive, setIsLevelSelectorActive } = useGameStore();


	return (
		<GameDialog open={isLevelSelectorActive} onOpenChange={setIsLevelSelectorActive} />
	)
}

export default LevelSelector;