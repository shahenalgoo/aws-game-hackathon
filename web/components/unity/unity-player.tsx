"use client";

import { FC, useEffect } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import { UnityLoader } from "./unity-loader";
import useAppStore from "@/store/useAppStore";

interface UnityPlayerProps {
	test?: string;
}

const UnityPlayer: FC<UnityPlayerProps> = () => {

	const { setIsUnityLoaded } = useAppStore();


	const {
		unityProvider,
		isLoaded,
		loadingProgression,
		// addEventListener,
		// removeEventListener,
		// sendMessage
	} = useUnityContext({
		loaderUrl: "/game/Build/b1.loader.js",
		dataUrl: "/game/Build/b1.data",
		frameworkUrl: "/game/Build/b1.framework.js",
		codeUrl: "/game/Build/b1.wasm",
		companyName: "AWS Hackathon",
		productName: "Blitzer Game",
		productVersion: "1"
	});

	useEffect(() => {
		setIsUnityLoaded(isLoaded)
	}, [isLoaded]);



	return (
		<>
			<UnityLoader isLoaded={isLoaded} loadingProgression={loadingProgression} />
			<Unity id="game" unityProvider={unityProvider} className="fixed top-0 left-0 aspect-video w-full h-screen" />
		</>
	);
}

export default UnityPlayer;