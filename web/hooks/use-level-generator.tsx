/**
 * LEVEL GENERATOR
 * Using AI
 */

import { Schema } from "@/amplify/data/resource";
import { generateClient } from "aws-amplify/api";

const client = generateClient<Schema>({ authMode: "userPool" });

const useLevelGenerator = () => {

	function generateInstructions() {
		// generate random seed number
		const seed = Math.floor(Math.random() * 1000);

		// define extraction point options
		const extractionPointOptions: string[] = [
			"top middle",
			"center middle",
			"bottom middle",
			"top right",
			"center right",
			"bottom right",
		];

		const instructions = "generate new level with seed number " + seed + ". Place the extraction point at the " + extractionPointOptions[seed % extractionPointOptions.length] + " of the grid";
		return instructions;
	}

	const generateLevel = async () => {
		console.log("🔃 Generating new level...");
		const { data } = await client.generations.GenerateLevels({ instructions: generateInstructions() });
		if (data) {
			console.log("✅ New level generated!");
			return data;
		}
	}

	return { generateLevel };

}

export default useLevelGenerator;