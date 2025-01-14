/**
 * Client side configuration for Amplify
 */

"use client";

import { Amplify } from "aws-amplify";
import outputs from "@/amplify_outputs.json";
import { generateClient } from "aws-amplify/api";
import { Schema } from "@/amplify/data/resource";

Amplify.configure(outputs, { ssr: true });

export default function ConfigureAmplifyClientSide() {
	return null;
}

export const client = generateClient<Schema>();