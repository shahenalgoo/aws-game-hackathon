import { PrismaAdapter } from "@auth/prisma-adapter";
import NextAuth from "next-auth";
// import Discord from "next-auth/providers/discord";
import GitHub from "next-auth/providers/github";
import { prisma } from "./prisma/client";

export const { handlers, signIn, signOut, auth } = NextAuth({
	adapter: PrismaAdapter(prisma),
	providers: [GitHub],
	// callbacks: {
	// 	async redirect({ url, baseUrl }) {
	// 		return url.startsWith(baseUrl) ? url : baseUrl;
	// 	},
	// }
})