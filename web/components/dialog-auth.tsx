"use client";

import { FC } from "react";
import useGameStore from "@/store/useGameStore";
import Dialog from "./dialog";
import useRefStore from "@/store/useRefStore";

interface LevelSelectorProps {

}

const LevelSelector: FC<LevelSelectorProps> = () => {


	const { isLevelSelectorActive, setIsLevelSelectorActive } = useGameStore();


	return (
		<Dialog open={isLevelSelectorActive} onOpenChange={setIsLevelSelectorActive} />
	)
}

export default LevelSelector;