import { FC, HTMLAttributes } from "react";
import Image from "next/image";

import useGameStore from "@/store/useGameStore";
import React from 'react';
import { Meteors } from "./meteors";
import { StarsBackground } from "./stars";
import { Separator } from "./ui/separator";
import { Button } from "./ui/button";
import { CircleUser, Trophy } from "lucide-react";

import useSound from 'use-sound';
const SOUND_URL = '/sfx/ui-click.wav';

interface MainMenuProps extends HTMLAttributes<HTMLDivElement> { }

const MainMenu: FC<MainMenuProps> = ({ children, ...props }) => {

	const { isMainMenuActive, setProfileDialogActive } = useGameStore();
	const [play] = useSound(SOUND_URL, { volume: 1 });

	const handleProfileDialog = () => {
		play();
		setProfileDialogActive(true);
	}

	if (!isMainMenuActive) return null;

	return (
		<div autoFocus className="fixed top-0 left-0 z-50 w-full h-full flex justify-center items-center bg-[radial-gradient(ellipse_at_top_left,_var(--tw-gradient-stops))] from-[#4F2C7D] via-[#200643] to-[#05001c]">

			<StarsBackground className="absolute z-10" />
			<div className="absolute top-0 right-0 w-full h-full -scale-x-100">
				<Meteors number={20} />
			</div>


			<div className="flex flex-col justify-center items-center">
				<Image src="/logo.png" alt="logo" width={720} height={215} className="w-auto h-auto max-w-xl mb-8 relative z-50" priority />
				{/* <h1 className="text-white text-4xl font-bold mb-4">Main Menu</h1> */}

				<div className="relative z-50 space-y-6">
					<div className="flex flex-col space-y-4">
						{children}
					</div>

					<Separator className="w-24 mx-auto" />

					<div className="flex gap-2">
						<Button onClick={handleProfileDialog} variant={"secondary"} size={"lg"} className="w-full">
							<CircleUser /> Sign In
						</Button>
						<Button variant={"secondary"} size={"lg"} className="w-full">
							<Trophy /> Leaderboard
						</Button>
					</div>

				</div>
			</div>
		</div>
	)
}

export default MainMenu;