'use client';

import { SessionProvider } from "next-auth/react";

export const ClientProviderSession = ({ children, }: { children: React.ReactNode }) => {
	return (
		<SessionProvider>
			{children}
		</SessionProvider>
	)
}