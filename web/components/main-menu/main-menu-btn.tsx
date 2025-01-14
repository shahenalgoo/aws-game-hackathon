/**
 * MENU BUTTONS
 * 
 */

"use client";

import { ButtonHTMLAttributes, FC } from "react";
import { MoveRight } from "lucide-react";
import useSound from 'use-sound';

const SOUND_URL = '/sfx/ui-click.wav';


interface MainMenuBtnProps extends ButtonHTMLAttributes<HTMLButtonElement> {
	title: string;
}


const MainMenuBtn: FC<MainMenuBtnProps> = ({ onClick, title }) => {

	const [play] = useSound(SOUND_URL, { volume: 1 });

	return (
		<button
			onClick={onClick}
			onMouseDown={() => play()}
			className="group w-full py-2 px-4 rounded-xl text-left transition-all border border-white/0 hover:border-primary bg-white/10 hover:bg-primary/5"
		>
			<div className={`flex items-center justify-between text-md tracking-normal transition-all group-hover:text-primary group-hover:tracking-wider`}>
				{title} <MoveRight strokeWidth={2} className="mr-10 opacity-0 transition-all group-hover:mr-0 group-hover:opacity-100" />
			</div>
		</button>
	)
}

export default MainMenuBtn;