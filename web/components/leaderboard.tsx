/**
 * LEADERBOARD COMPONENT
 * This is a basic implementation for the hackathon,
 * in a real world situation we would work on making 
 * the leaderboard more efficient with caching and pagination.
 * 
 */

"use client";

import { useEffect, useState } from "react";
import { useApplicationStore } from "@/store/use-application-store";
import { Schema } from "@/amplify/data/resource";
import { client } from "./amplify/amplify-client-config";
import { formatTime } from "@/lib/format-time";
import Dialog from "./ui/dialog";
import { Button } from "./ui/button";
import { ScrollArea } from "./ui/scroll-area";
import { ChevronLeft } from "lucide-react";


type ActiveTab = "normal" | "bossFight" | "survival";


const Leaderboard = () => {


	// Global Store
	const { leaderboardDialogActive, setLeaderboardDialogActive } = useApplicationStore();


	// Local States
	const [activeTab, setActiveTab] = useState<ActiveTab>("normal");
	const [activeLeaderboard, setActiveLeaderboard] = useState<Array<Schema["Leaderboard"]["type"]>>([]);
	const [leaderboardCampaign, setLeaderboardCampaign] = useState<Array<Schema["Leaderboard"]["type"]>>([]);
	const [leaderboardBossFight, setLeaderboardBossFight] = useState<Array<Schema["Leaderboard"]["type"]>>([]);
	const [leaderboardSurvival, setLeaderboardSurvival] = useState<Array<Schema["Leaderboard"]["type"]>>([]);


	// Common stylings
	const styles = {
		button: `py-1 px-3 rounded-full text-sm font-semibold text-muted-foreground`,
		activeButton: `bg-primary !text-black`,
	}


	// FETCH LEADERBOARD
	// Fetch leaderboard on first load to better experience.
	useEffect(() => {
		const sub = client.models.Leaderboard.observeQuery().subscribe({
			next: (data) => {
				const sorted = data.items.sort((a, b) => a.time - b.time);

				// Campaign Leaderboard
				const campaignData = sorted.filter((item) => item.mode === "normal");
				if (activeTab === "normal") setActiveLeaderboard(campaignData);

				// Boss Fight Leaderboard
				const bossFightData = sorted.filter((item) => item.mode === "bossFight");
				setLeaderboardBossFight([...bossFightData]);
				if (activeTab === "bossFight") setActiveLeaderboard(bossFightData);

				// Survival Leaderboard
				const survivalData = sorted.filter((item) => item.mode === "survival");
				const survivalDataByRound = survivalData.sort((a, b) => {
					if (a.round === null && b.round === null) return 0;
					if (a.round === null) return 1;
					if (b.round === null) return -1;
					return b.round - a.round;
				});

				setLeaderboardSurvival([...survivalDataByRound]);
				if (activeTab === "survival") setActiveLeaderboard(survivalDataByRound);
			},
		});

		return () => sub.unsubscribe();
	}, [activeTab]);


	// Handles the active leaderboard tab change
	const handleChangeActiveLeaderboard = (activeTab: ActiveTab) => {
		setActiveTab(activeTab);

		switch (activeTab) {
			case "normal":
				setActiveLeaderboard(leaderboardCampaign);
				break;

			case "bossFight":
				setActiveLeaderboard(leaderboardBossFight);
				break;

			case "survival":
				setActiveLeaderboard(leaderboardSurvival);
				break;
		}
	}


	// For development purposes only
	// async function handleDeleteLeaderboardEntry(id: any) {
	// 	await client.models.Leaderboard.delete({ id })
	// }


	// For development purposes only
	// async function createEntry() {
	// 	const { errors } = await client.models.Leaderboard.create({
	// 		userId: "dasdaxxxxxxxx",
	// 		username: "DeadShotter2",
	// 		mode: "bossFight",
	// 		time: 1752.62,
	// 		// round: 13
	// 	});
	// }

	return (
		<Dialog
			open={leaderboardDialogActive}
			onOpenChange={setLeaderboardDialogActive}
			className="overflow-hidden max-w-2xl h-[calc(100vh_-_10rem)]"
		>

			<div className="relative z-10">

				{/* LEADERBOARD HEADER */}
				<div className="mb-8 flex items-center">
					<Button variant={"outline"} size={"icon"} className="mr-4" onClick={() => setLeaderboardDialogActive(false)}>
						<ChevronLeft />
					</Button>
					<h3 className="font-orbitron text-xl tracking-wider">Leaderboards</h3>
					<div className="bg-secondary rounded-full p-1 ml-auto">
						<button className={`${styles.button} ${activeTab === "normal" && styles.activeButton}`} onClick={() => handleChangeActiveLeaderboard("normal")}>Campaign</button>
						<button className={`${styles.button} ${activeTab === "bossFight" && styles.activeButton}`} onClick={() => handleChangeActiveLeaderboard("bossFight")}>Boss Fight</button>
						<button className={`${styles.button} ${activeTab === "survival" && styles.activeButton}`} onClick={() => handleChangeActiveLeaderboard("survival")}>Survival</button>
					</div>
				</div>

				{/* <button onClick={createEntry}>Create Entry</button> */}

				{/* LEADERBOARD TABLE HEAD */}
				<div className="mb-4 px-4">
					<div className={`grid text-sm text-muted-foreground border px-4 py-2 rounded-xl bg-secondary/20 ${activeTab === "survival" ? "grid-cols-[4rem_1fr_7rem_6rem]" : "grid-cols-[4rem_1fr_6rem]"}`}>
						<div>Rank</div>
						<div>Player</div>
						{activeTab === "survival" && <div>Rounds</div>}
						<div className="text-right">Time</div>
					</div>
				</div>


				{/* LEADERBOARD ENTRIES */}
				<ScrollArea className="h-[calc(100vh_-_21rem)] px-4">
					<div className="space-y-2">
						{activeLeaderboard.map((item, index) => (
							<div key={item.id} className={`grid ${activeTab === "survival" ? "grid-cols-[4rem_1fr_3rem_10rem]" : "grid-cols-[4rem_1fr_10rem]"}  items-center border px-4 py-2 rounded-xl text-sm`}>
								<div>#{index + 1}</div>
								<div>{item.username}</div>
								{activeTab === "survival" && <div>{item.round}</div>}
								<div className="text-right">{formatTime(item.time)}</div>
								{/* <button onClick={() => handleDeleteLeaderboardEntry(item.id)}>Delete</button> */}
							</div>
						))}
					</div>
				</ScrollArea>

			</div>
		</Dialog>
	)
}

export default Leaderboard;