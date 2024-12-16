"use client";

import { signOut, useSession } from "next-auth/react";
import { FC } from "react";
import { Button } from "../ui/button";

interface LoginProps {
	string?: string;
}

const Login: FC<LoginProps> = () => {

	const { data: session } = useSession();

	const handleSignIn = async (provider: string) => {
		window.open(
			`/auth-social/${provider}`,
			"popup",
			"width=600,height=600"
		);
	};


	return (
		<>
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
		</>
	);
}

export default Login;