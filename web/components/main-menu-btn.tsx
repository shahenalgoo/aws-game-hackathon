"use client";

import { ButtonHTMLAttributes, FC, useState } from "react";

import { Orbitron } from "next/font/google";
const orbitron = Orbitron({
	weight: '500',
	subsets: ['latin'],
	display: 'swap',
});

import { MoveRight } from "lucide-react";

import useSound from 'use-sound';
const SOUND_URL = '/sfx/ui-click.wav';

interface MainMenuButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
	title: string;
	description: string;
}

const MainMenuButton: FC<MainMenuButtonProps> = ({ onClick, title, description }) => {

	const [isHovering, setIsHovering] = useState(false);
	const [play, { stop }] = useSound(SOUND_URL, { volume: 1 });


	return (
		<>
			<button
				onClick={onClick}
				onMouseDown={() => {
					play()
				}}
				className="group w-[22rem] space-y-1 py-4 px-6 rounded-2xl text-left transition-all border border-white/10 hover:border-primary bg-white/10 hover:bg-primary/5"
			>
				<div className={`flex items-center justify-between ${orbitron.className} text-2xl tracking-normal transition-all group-hover:text-primary group-hover:tracking-wider`}>
					{title} <MoveRight strokeWidth={2} className="mr-10 opacity-0 transition-all group-hover:mr-0 group-hover:opacity-100" />
				</div>
				<div className="transition-all text-white/70 group-hover:text-white">{description}</div>
			</button>

		</>
	)
}

export default MainMenuButton;