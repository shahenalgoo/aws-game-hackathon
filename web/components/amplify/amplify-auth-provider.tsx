/**
 * This is a wrapper component that allows the Amplify Authenticator component
 */

"use client";

import { Authenticator } from "@aws-amplify/ui-react";
import "@aws-amplify/ui-react/styles.css";
export default function AmplifyAuthProvider({
	children,
}: {
	children: React.ReactNode;
}) {
	return (
		<Authenticator signUpAttributes={["preferred_username"]} className="relative z-50">
			{children}
		</Authenticator>
	)
}