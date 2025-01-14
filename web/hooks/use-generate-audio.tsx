/**
 * GENERATE AUDIO USING AWS POLLY
 * 
 */

import useOverlordStore from "@/store/use-overlord-store";

const useGenerateAudio = () => {

	const { setAudio, stopOverlordAudio } = useOverlordStore();

	const generateAudio = async (
		text: string,
		oncanplaythrough?: () => void,
		onended?: () => void
	) => {
		try {
			const response = await fetch('/api/text-to-speech', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
				},
				body: JSON.stringify({ text }),
			});

			if (!response.ok) {
				throw new Error('Failed to generate speech');
			}

			const audioBlob = await response.blob();
			const audioUrl = URL.createObjectURL(audioBlob);
			const audio = new Audio(audioUrl);

			stopOverlordAudio();

			// Wait for audio to be loaded before playing
			await new Promise((resolve, reject) => {
				// Ensure audio is preloaded
				audio.preload = 'auto';

				// Handle successful loading
				audio.oncanplaythrough = () => {
					if (oncanplaythrough) {
						oncanplaythrough();
					}
					audio.play()
						.then(resolve)
						.catch(reject);

				};

				// Handle loading error
				audio.onerror = (error) => {
					reject(error);
				};

				// Clean up the URL object when done and invoke the passed onended callback
				audio.onended = () => {
					URL.revokeObjectURL(audioUrl);
					// Call the custom onended callback if provided
					if (onended) {
						onended();
					}
				};

				// Start loading the audio
				audio.load();

			});
			setAudio(audio);

		} catch (error) {
			console.error('Error:', error);
		}
	}

	return { generateAudio };
}

export default useGenerateAudio;
