/**
 * MENU SECTIONS
 * 
 */

"use client";

import { FC } from "react";

interface MainMenuSectionProps {
	title: string;
	children: React.ReactNode;
}

const MainMenuSection: FC<MainMenuSectionProps> = ({ title, children }) => {
	return (
		<div className="relative z-50 w-96 px-2 py-3 space-y-2 rounded-2xl border-t border-l border-r border-white/10">
			<h2 className="absolute -top-3 left-4">
				<span className="px-2 rounded-full bg-black font-medium text-xs text-primary uppercase">{title}</span>
			</h2>
			{children}
		</div>
	)
}


interface MainMenuSectionPrimaryProps {
	children: React.ReactNode;
}

const MainMenuSectionPrimary: FC<MainMenuSectionPrimaryProps> = ({ children }) => {
	return children;
}


interface MainMenuSectionSecondaryProps {
	children: React.ReactNode;
}

const MainMenuSectionSecondary: FC<MainMenuSectionSecondaryProps> = ({ children }) => {
	return (
		<div className="flex items-center gap-2">
			{children}
		</div>
	)
}

export { MainMenuSection, MainMenuSectionPrimary, MainMenuSectionSecondary };
