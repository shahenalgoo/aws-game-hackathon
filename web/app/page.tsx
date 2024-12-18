"use client";

import { Button } from "@/components/ui/button";
import { CircleUser, Fullscreen } from "lucide-react";
// import Login from "@/components/auth/login";
import React, { useEffect, useRef } from "react";

import {
	Sheet,
	SheetContent,
	SheetDescription,
	SheetHeader,
	SheetTitle,
	SheetTrigger,
} from "@/components/ui/sheet"
import UnityPlayer from "@/components/unity/unity-player";
import useGameStore from "@/store/useGameStore";

// import useSound from 'use-sound';
// const soundUrl = '/music/main-menu.mp3';

// const fetchTestApi = async () => {
// 	const res = await fetch("http://localhost:3000/api/test");
// 	return await res.json();
// };

export default function Home() {

	const containerRef = useRef<HTMLDivElement>(null);

	const { isUnityLoaded } = useGameStore();

	// const [isLoaded, setIsLoaded] = React.useState(false);
	// const [isPlaying, setIsPlaying] = React.useState(false);
	// const [play, { stop }] = useSound(soundUrl, {
	// 	onload: () => setIsLoaded(true),
	// 	onend: () => setIsPlaying(false),
	// });

	// const [isPaused, setIsPaused] = useState(false);


	// useEffect(() => {
	// 	play();
	// 	setIsPlaying(true);
	// }, [isLoaded]);




	const handleFullscreen = () => {
		if (containerRef.current) {
			if (document.fullscreenElement) {
				document.exitFullscreen();
			} else {
				containerRef.current.requestFullscreen();
			}
		}
	};


	// // Handle browser focus and blur events
	// useEffect(() => {
	// 	const handleBlur = () => {
	// 		setIsPaused(true); // Show the pause overlay
	// 	};

	// 	const handleFocus = () => {
	// 		setIsPaused(false); // Hide the pause overlay
	// 	};

	// 	window.addEventListener("blur", handleBlur);
	// 	window.addEventListener("focus", handleFocus);

	// 	return () => {
	// 		window.removeEventListener("blur", handleBlur);
	// 		window.removeEventListener("focus", handleFocus);
	// 	};
	// }, []);

	// const handleResume = () => {
	// 	setIsPaused(false); // Manually resume
	// };

	return (
		<div className="relative w-full h-screen" ref={containerRef}>
			{/* <Login />
			<hr className="my-4" /> */}
			<UnityPlayer />

			{/* Pause Overlay */}
			{/* {isPaused && (
				<div className="absolute top-0 left-0 w-full h-full bg-black bg-opacity-50 flex flex-col justify-center items-center z-10">
					<h1 className="text-white text-4xl font-bold mb-4">Game Paused</h1>
					<button
						onClick={handleResume}
						className="px-4 py-2 bg-green-500 text-white text-lg rounded"
					>
						Resume
					</button>
				</div>
			)} */}

			{isUnityLoaded &&
				<div className="fixed z-50 top-0 right-0 flex gap-1 p-1">



					<Sheet>
						<SheetTrigger asChild>
							<Button variant={"secondary"} size={"icon"}>
								<CircleUser />
							</Button>
						</SheetTrigger>


						<SheetContent ref={containerRef} className="w-full !max-w-lg">
							<SheetHeader>
								<SheetTitle>Are you absolutely sure?</SheetTitle>
								<SheetDescription>
									This action cannot be undone. This will permanently delete your account
									and remove your data from our servers.
								</SheetDescription>
							</SheetHeader>
						</SheetContent>

					</Sheet>


					<Button variant={"secondary"} size={"icon"} onClick={handleFullscreen}>
						<Fullscreen />
					</Button>
				</div>
			}

		</div>
	);
}
