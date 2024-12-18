"use client";

import useRefStore from "@/store/useRefStore";
import { Button } from "./ui/button";
import { Fullscreen } from "lucide-react";


const FullscreenMode = () => {

	const { containerRef } = useRefStore();

	const handleFullscreen = () => {
		if (!containerRef || !containerRef.current) return;

		if (document.fullscreenElement) {
			document.exitFullscreen();
		} else {
			containerRef.current.requestFullscreen();
		}
	};


	return (
		<Button variant={"secondary"} size={"icon"} onClick={handleFullscreen}>
			<Fullscreen />
		</Button>
	);
}

export default FullscreenMode;