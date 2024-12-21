import { NextResponse } from "next/server";
import { prisma } from "@/prisma/client";

export async function GET() {
	const response = await prisma.leaderboard.findMany({
		include: {
			user: {
				select: {
					username: true,
					image: true,
				}
			}
		},
	});
	return NextResponse.json(response);

}