"use client";

import { useEffect } from "react";
import { signIn, useSession } from "next-auth/react";
import { usePathname } from "next/navigation";

export default function Home() {

	const pathname = usePathname();
	const { data: session, status } = useSession();


	useEffect(() => {
		if (!(status === "loading") && !session) return;

		const provider = pathname?.split('/').pop();
		if (!provider) return;

		signIn(provider, {
			redirect: false
		});
	}, [pathname]);

	useEffect(() => {
		if (session) window.close();
	}, [session]);


	return (
		<div className="w-full h-screen flex items-center justify-center">
			Preparing to sign in...
		</div>
	);
}
