import "./globals.css";
import { Saira } from "next/font/google";
import type { Metadata } from "next";
import ConfigureAmplifyClientSide from "@/components/amplify/amplify-client-config";
import AmplifyAuthProvider from "@/components/amplify/amplify-auth-provider";
import { StarsBackground } from "@/components/ui/stars-background";

const saira = Saira({ subsets: ["latin"] });

export const metadata: Metadata = {
	title: "Play Blitzer",
	description: "A video game designed for AWS Game Builder Challenge",
};

export default function RootLayout({
	children,
}: {
	children: React.ReactNode;
}) {
	return (
		<html lang="en" className="dark" suppressHydrationWarning>
			<body className={`${saira.className} overflow-hidden flex justify-center items-center`}>
				<StarsBackground className="absolute top-0 left-0 z-0 w-full h-full" />
				<ConfigureAmplifyClientSide />
				<AmplifyAuthProvider>
					{children}
				</AmplifyAuthProvider>
			</body>
		</html>
	);
}
