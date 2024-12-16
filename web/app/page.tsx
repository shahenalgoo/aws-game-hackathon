"use client";

import { Button } from "@/components/ui/button";
import { useSession } from "next-auth/react";
import { signOut } from "next-auth/react";

export default function Home() {

	const { data: session } = useSession();

	const handleSignIn = async (provider: string) => {
		window.open(
			`/auth-social/${provider}`,
			"popup",
			"width=600,height=600"
		);
	};


	return (
		<div>
			Hello {session?.user?.name} <br />
			<Button
				onClick={() => handleSignIn("github")}
			>
				Sign in with Github
			</Button>

			<Button
				onClick={() => signOut()}
			>
				Sign Out
			</Button>
		</div>
	);
}
