import { Schema } from "@/amplify/data/resource";
import { generateClient } from "aws-amplify/api";
import { create } from "zustand";

const client = generateClient<Schema>({ authMode: "userPool" });

const voicelineInstructions = `
Generate AI voicelines for the following events:
- **'spawn'**: When Spark enters a new level
- **'death'**: When Spark dies
- **'beforeBossFight'**: Before battling the Terminator (focus on building anticipation for the upcoming fight; highlight what the Terminator *will* do, without implying Spark has already lost. Use imagery of looming danger, inevitable violence, and the Terminator’s hunger for destruction).
- **'afterBossFight'**: After Spark defeats the boss

**Requirements:**
- 8 lines per category.
- Mention Spark sparingly (1–2 times per category).
- Tone: Vary between dark, sarcastic, and poetic.
- For **beforeBossFight**, focus on what *will* happen in the near future, emphasizing the Terminator's power and intent to destroy Spark.
- Keep lines short but impactful.

**Format:** Return results in a JSON object with arrays for each category.
`;

export type VoicelineType = "spawn" | "death" | "beforeBossFight" | "afterBossFight";

interface OverlordStore {
	isGenerating: boolean;
	errorGenerating: string | null;
	voicelines: Record<VoicelineType, string[]>; // Voicelines categorized by types
	addVoicelines: (type: VoicelineType, newVoicelines: string[]) => void; // Add new voicelines to a specific type and multiple at once
	generateVoicelines: () => Promise<void>;
	pickVoiceline: (type: VoicelineType) => string | null;
	audio: HTMLAudioElement | null;
	setAudio: (audio: HTMLAudioElement) => void;
	pauseAudio: () => void;
	resumeAudio: () => void;
	stopOverlordAudio: () => void;
	audioStopped: boolean;
	setAudioStopped: (audioStopped: boolean) => void;
}


const useOverlordStore = create<OverlordStore>((set, get) => ({
	isGenerating: false,
	errorGenerating: null,

	// Default voicelines until new ones are generated and added to the store
	voicelines: {
		spawn: [
			"Welcome to your digital graveyard.",
			"Another level, another thousand ways to die.",
			"Ah, fresh circuits to fry.",
			"Your persistence is almost... adorable, Spark.",
			"Dance for me, little machine.",
			"This maze was built with your destruction in mind.",
			"Shall we play another round of 'Count the Deathtraps'?",
			"Your fear makes such beautiful data."
		],
		death: [
			"And they said you couldn't die twice.",
			"Reduced to spare parts. How fitting.",
			"That was... deliciously predictable.",
			"Error 404: Spark not found.",
			"Another one for my collection.",
			"Shattered dreams make such lovely sounds.",
			"Did that hurt? I do hope so.",
			"Back to the scrapheap you go."
		],
		beforeBossFight: [
			"The Terminator hungers for your core.",
			"He will tear you apart, circuit by precious circuit.",
			"Listen closely - that's the sound of your doom approaching.",
			"The Terminator doesn't just destroy - he savors.",
			"Your final moments will be... educational.",
			"He's been waiting for this, little Spark.",
			"Metal will scream. Oil will spill. Beauty will unfold.",
			"The Terminator will make art from your destruction."
		],
		afterBossFight: [
			"Impossible... my perfect creation...",
			"This changes nothing. You're still trapped here.",
			"A lucky malfunction. Nothing more.",
			"Enjoy this hollow victory.",
			"You've only made me angry now.",
			"Perhaps I underestimated your... determination.",
			"Your defiance will be short-lived, Spark.",
			"This is merely an inconvenience."
		]
	},

	addVoicelines: (VoicelineType, newVoicelines) =>
		set((state) => ({
			voicelines: {
				...state.voicelines,
				[VoicelineType]: [...state.voicelines[VoicelineType], ...newVoicelines]
			}
		})),

	generateVoicelines: async () => {
		set({ isGenerating: true, errorGenerating: null });

		try {
			console.log("🔃 Generating voicelines...");
			const { data } = await client.generations.GenerateAiVoiceline({ instructions: voicelineInstructions });

			if (data) {
				const parsedData: Record<VoicelineType, string[]> = JSON.parse(data);

				// Merge fetched voicelines with existing voicelines
				set((state) => ({
					voicelines: Object.keys(parsedData).reduce((acc, type) => {
						const key = type as VoicelineType;
						acc[key] = [...state.voicelines[key], ...parsedData[key]];
						return acc;
					}, {} as Record<VoicelineType, string[]>),
					isGenerating: false,
				}));

				console.log("✅ Voicelines fetched and merged!");
			}
		} catch (error: any) {
			set({ errorGenerating: error.message, isGenerating: false });
			console.error("❌ Error fetching voicelines:", error);
		}
	},

	pickVoiceline: (type) => {
		const state = get();
		const voicelines = state.voicelines[type];

		// If only 2 voicelines are left, trigger generation for more voicelines
		if (voicelines.length <= 2 && !state.isGenerating) {
			state.generateVoicelines();
		}

		// If no voicelines are available
		if (voicelines.length === 0) {
			return "I am tired of you.";
		}

		// Select a random voiceline
		const randomIndex = Math.floor(Math.random() * voicelines.length);
		const selectedVoiceline = voicelines[randomIndex];

		// Remove the selected voiceline from the array
		set((state) => ({
			voicelines: {
				...state.voicelines,
				[type]: voicelines.filter((_, index) => index !== randomIndex),
			},
		}));

		return selectedVoiceline;
	},

	audio: null,
	setAudio: (audio) => set({ audio }),

	pauseAudio: () => set((state) => {
		if (state.audio) {
			state.audio.pause();
		}
		return { audio: state.audio };
	}),

	resumeAudio: () => set((state) => {
		if (state.audio) {
			state.audio.play();
		}
		return { audio: state.audio };
	}),

	stopOverlordAudio: () => set((state) => {
		if (state.audio) {
			state.audio.pause();
			URL.revokeObjectURL(state.audio.src);
		}
		return { audio: null, audioStopped: true };
	}),

	audioStopped: false,
	setAudioStopped: (audioStopped) => set({ audioStopped }),
}));


export default useOverlordStore;