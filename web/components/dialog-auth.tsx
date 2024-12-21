"use client";

import { FC } from "react";
import useGameStore from "@/store/useGameStore";
import Dialog from "./dialog";
import { signOut, useSession } from "next-auth/react";
import { Button } from "./ui/button";
import { PiDiscordLogoDuotone, PiGithubLogoDuotone } from "react-icons/pi";

interface DialogAuthProps {

}

const DialogAuth: FC<DialogAuthProps> = () => {

	const { profileDialogActive, setProfileDialogActive } = useGameStore();
	const { data: session } = useSession();

	const handleSignIn = async (provider: string) => {
		window.open(
			`/auth-social/${provider}`,
			"popup",
			"width=600,height=600"
		);
	};


	return (
		<Dialog
			open={profileDialogActive}
			onOpenChange={setProfileDialogActive}
			title={!session ? "Sign In" : "My Account"}
		>
			{/* Hello {session?.user?.name} <br /> */}

			{!session &&
				<div className="grid grid-cols-2 gap-4">
					<Button
						variant={"outline"}
						onClick={() => handleSignIn("discord")}
					>
						<PiDiscordLogoDuotone size={22} /> Discord
					</Button>

					<Button
						variant={"outline"}
						onClick={() => handleSignIn("twitter")}
					>
						<PiDiscordLogoDuotone size={22} /> Twitter
					</Button>

					<Button
						variant={"outline"}
						onClick={() => handleSignIn("github")}
					>
						<PiGithubLogoDuotone /> Github
					</Button>
				</div>
			}

			{session &&
				<Button
					variant={"destructive"}
					onClick={() => signOut()}
				>
					Sign Out
				</Button>
			}
		</Dialog>
	)
}

export default DialogAuth;