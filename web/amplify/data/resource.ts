import { type ClientSchema, a, defineData } from "@aws-amplify/backend";
import { promptLevels } from "./prompt-levels";
import { promptAiVoicelines } from "./prompt-ai-voicelines";

const schema = a.schema({

	GenerateLevels: a.generation({
		aiModel: a.ai.model('Claude 3.5 Sonnet v2'),
		systemPrompt: promptLevels,
		inferenceConfiguration: {
			maxTokens: 1000,
			temperature: 0.5,
			topP: 0.9,
		}
	})
		.arguments({
			instructions: a.string(),
		})
		.returns(
			a.string()
		)
		.authorization((allow) => allow.authenticated()),



	GenerateAiVoiceline: a.generation({
		aiModel: a.ai.model('Claude 3.5 Sonnet v2'),
		systemPrompt: promptAiVoicelines,
		inferenceConfiguration: {
			maxTokens: 2000,
			temperature: 1,
			topP: 0.9,
		}
	})
		.arguments({
			instructions: a.string(),
		})
		.returns(
			a.string()
		)
		.authorization((allow) => allow.authenticated()),



	AiLevel: a.model({
		grid: a.string(),
		generatedBy: a.string(),
		cover: a.string(),
	})
		.authorization((allow) => [allow.authenticated().to(['create', 'read'])]),



	Leaderboard: a
		.model({
			userId: a.string().required(),
			username: a.string().required(),
			mode: a.enum(["normal", "bossFight", "survival"]),
			time: a.float().required(),
			round: a.integer()
		})
		.authorization((allow) => [allow.authenticated().to(['create', 'read', 'update'])]),
});

export type Schema = ClientSchema<typeof schema>;

export const data = defineData({
	schema,
	authorizationModes: {
		defaultAuthorizationMode: "userPool",
	},
});