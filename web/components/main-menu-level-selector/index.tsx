"use client";

import { FC } from "react";
import {
	Dialog,
	DialogContent,
	DialogDescription,
	DialogHeader,
	DialogTitle,
	DialogTrigger,
} from "@/components/ui/dialog"
import useGameStore from "@/store/useGameStore";

interface LevelSelectorProps {

}

const LevelSelector: FC<LevelSelectorProps> = () => {

	const { isLevelSelectorActive, setIsLevelSelectorActive } = useGameStore();


	return (
		<Dialog open={isLevelSelectorActive} onOpenChange={setIsLevelSelectorActive}>
			<DialogContent>
				<DialogHeader>
					<DialogTitle>Are you absolutely sure?</DialogTitle>
					<DialogDescription>
						This action cannot be undone. This will permanently delete your account
						and remove your data from our servers.
					</DialogDescription>
				</DialogHeader>
			</DialogContent>
		</Dialog>
	)
}

export default LevelSelector;