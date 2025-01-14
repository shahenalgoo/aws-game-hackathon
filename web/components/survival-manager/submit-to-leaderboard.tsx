/**
 * SUBMIT SURVIVAL SCORE TO LEADERBOARD
 * 
 */

"use client";

import { FC, useCallback, useEffect, useState } from "react";
import Dialog from "../ui/dialog";
import { useSurvivalModeStore } from "@/store/use-survival-mode-store";
import { ReactUnityEventParameter } from "react-unity-webgl/distribution/types/react-unity-event-parameters";
import useOverlordStore from "@/store/use-overlord-store";
import { Button } from "../ui/button";
import { formatTime } from "@/lib/format-time";
import { Separator } from "../ui/separator";
import { useAuthenticator } from "@aws-amplify/ui-react";
import { fetchUserAttributes } from "aws-amplify/auth";
import { client } from "../amplify/amplify-client-config";
import { Loader } from "lucide-react";

interface SurvivalSubmitToLeaderboardProps {
	addEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	removeEventListener: (eventName: string, callback: (...parameters: ReactUnityEventParameter[]) => ReactUnityEventParameter) => void;
	sendMessage: (gameObjectName: string, methodName: string, parameter?: ReactUnityEventParameter) => void;
}


const SurvivalSubmitToLeaderboard: FC<SurvivalSubmitToLeaderboardProps> = ({ addEventListener, removeEventListener, sendMessage }) => {


	// Global Store
	const { submitDialogActive, setSubmitDialogActive } = useSurvivalModeStore();
	const { stopOverlordAudio, setAudioStopped } = useOverlordStore();


	// Local States
	const [isSubmitting, setIsSubmitting] = useState(false);
	const [submissionSuccess, setSubmissionSuccess] = useState(false);
	const [submissionError, setSubmissionError] = useState(false);
	const [errorMessage, setErrorMessage] = useState<string>("An unknown error occured.");
	const [submissionData, setSubmissionData] = useState({
		time: 0,
		round: 1
	});


	// Hooks
	const { user } = useAuthenticator();


	// Listen to the submission event
	const receivedSubmissionEvent = useCallback((time: any, round: any) => {
		setSubmitDialogActive(true);
		setSubmissionData({ time, round });
	}, []);

	useEffect(() => {
		addEventListener("SubmitSurvivalData", receivedSubmissionEvent);
		return () => removeEventListener("SubmitSurvivalData", receivedSubmissionEvent);
	}, [addEventListener, removeEventListener, receivedSubmissionEvent]);


	// Submit score to leaderboard
	useEffect(() => {
		if (submitDialogActive && submissionData.time && submissionData.round) {
			handleSubmitToLeaderboard(submissionData.time, submissionData.round);
		}
	}, [submitDialogActive, submissionData]);


	// Compare submission entry with existing entry
	const compareLeaderboardEntry = (
		newEntry: { round: number; time: number },
		existingEntry: { round: number; time: number }
	): { shouldUpdate: boolean; message?: string } => {
		// Compare rounds first
		if (newEntry.round > existingEntry.round) {
			// New round is better (higher), update regardless of time
			return {
				shouldUpdate: true,
				message: "New high round achieved!"
			};
		}

		if (newEntry.round === existingEntry.round) {
			// Rounds are equal, compare times
			if (newEntry.time >= existingEntry.time) {
				return {
					shouldUpdate: false,
					message: "Your previous time for this round was better!"
				};
			}
			return {
				shouldUpdate: true,
				message: "New best time for this round!"
			};
		}

		// New round is worse (lower)
		return {
			shouldUpdate: false,
			message: "You reached a higher round before!"
		};
	};


	// Submit score to leaderboard
	const handleSubmitToLeaderboard = async (time: any, round: any) => {

		// Prevent duplicate submissions
		if (isSubmitting || submissionSuccess) return;

		setIsSubmitting(true);

		try {
			const username = await fetchUserAttributes().then((user) => user.preferred_username);

			if (!username) {
				throw new Error("Username not found when submitting to leaderboard.");
			}

			// Check if user already submitted to leaderboard
			const { data: existingEntry } = await client.models.Leaderboard.list({
				filter: {
					userId: { eq: user.userId },
					mode: { eq: "survival" }
				}
			});

			if (existingEntry?.length > 0 && existingEntry[0].round) {

				const comparison = compareLeaderboardEntry(
					{ round, time },
					{ round: existingEntry[0].round, time: existingEntry[0].time }
				);

				if (!comparison.shouldUpdate) {
					throw new Error(comparison.message);
				}

				// Update existing entry with better time
				const { data, errors } = await client.models.Leaderboard.update({
					id: existingEntry[0].id,
					time: time,
					round: round
				});

				if (data) {
					console.log("✅ data updated", data);
				}

				if (errors) {
					throw new Error("Error updating your entry.");
				}

				setIsSubmitting(false);
				setSubmissionSuccess(true);
				return;
			}

			// Create new entry if none exists
			else {
				const { data, errors } = await client.models.Leaderboard.create({
					userId: user.userId,
					username: username,
					mode: "survival",
					time: time,
					round: round
				});

				if (data) {
					console.log("✅ data", data);
				}

				if (errors) {
					throw new Error("Error creating your entry.");
				}

				setSubmissionSuccess(true);
			}
		} catch (error) {
			setSubmissionError(true);
			setErrorMessage(error instanceof Error ? error.message : "An unknown error occurred.");
		} finally {
			setIsSubmitting(false);
		}
	};


	// Exit Game
	function handleExitGame() {
		stopOverlordAudio();
		setAudioStopped(true);
		setSubmitDialogActive(false);
		setSubmissionSuccess(false);
		setSubmissionError(false);
		setErrorMessage("An unknown error occured.");
		setSubmissionData({ time: 0, round: 0 });
		sendMessage("UICanvas", "ExitToMainMenu");
	}

	return submitDialogActive ? (
		<Dialog
			open={submitDialogActive}
			onOpenChange={() => { }}
			className="overflow-hidden max-w-xs"
		>
			<div className="space-y-4 relative z-10 flex flex-col items-center justify-center">

				<div className="text-center">
					<h2 className="text-3xl font-orbitron font-medium tracking-wide text-rose-500">You Died</h2>
					<span className="text-sm text-muted-foreground flex items-center">
						{isSubmitting && <><Loader size={16} className="mr-2 animate-spin" /> Submitting to Leaderboard</>}
						{submissionSuccess && <>Submission Successful!</>}
						{submissionError && <>{errorMessage}</>}
					</span >
				</div>

				<div className="w-full space-y-2 px-4 py-3 border rounded-2xl bg-white/5 text-center">
					<div className="flex items-center justify-between">
						<span className="text-muted-foreground">Rounds Survived</span>
						<span>{submissionData.round}</span>
					</div>
					<Separator />
					<div className="flex items-center justify-between">
						<span className="text-muted-foreground">Time Spent</span>
						<span>{formatTime(submissionData.time, true)}</span>
					</div>
				</div>

				{!isSubmitting &&
					<div className="space-y-3 outline-none">
						<Button onClick={handleExitGame} variant={"secondary"} className="w-full" tabIndex={-1}>Exit to Main Menu</Button>
					</div>
				}
			</div>
		</Dialog>
	) : null;
}

export default SurvivalSubmitToLeaderboard;