import { defineStorage } from '@aws-amplify/backend';

export const storage = defineStorage({
	name: 'blitzerGameDrive',
	access: (allow) => ({
		'ai-levels/*': [
			allow.authenticated.to(["read", "write"])
		]
	})
});