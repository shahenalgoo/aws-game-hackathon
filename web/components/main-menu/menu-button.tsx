"use client";

import { ButtonHTMLAttributes, FC, useState } from "react";

import { Orbitron } from "next/font/google";
const orbitron = Orbitron({
	weight: '500',
	subsets: ['latin'],
	display: 'swap',
});

import useSound from 'use-sound';
const SOUND_URL = '/sfx/ui-hover.wav';

interface MenuButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
	title: string;
	description: string;
}

const MenuButton: FC<MenuButtonProps> = ({ onClick, title, description }) => {

	const [isHovering, setIsHovering] = useState(false);
	const [play, { stop }] = useSound(SOUND_URL, { volume: 1 });


	return (
		<>
			<button
				onClick={onClick}
				// onMouseEnter={() => {
				// 	setIsHovering(true);
				// 	play();
				// }}
				// onMouseLeave={() => {
				// 	setIsHovering(false);
				// 	stop();
				// }}
				onMouseDown={() => {
					play()
				}}
				className="group w-[22rem] space-y-1 py-4 px-6 rounded-2xl text-left transition-all border border-white/10 hover:border-primary bg-white/10 hover:bg-primary/5"
			>
				<div className={`${orbitron.className} text-2xl transition-all group-hover:text-primary`}>{title}</div>
				<div className="transition-all text-white/70 group-hover:text-white">{description}</div>
			</button>

		</>
	)
}

export default MenuButton;