/**
 * DIALOG
 * A custom dialog component made with radix-ui
 * 
 */

"use client";

import { FC } from "react";
import { useRefStore } from "@/store/use-ref-store";
import { cn } from "@/lib/utils";
import * as DialogPrimitive from "@radix-ui/react-dialog";
import * as VisuallyHidden from "@radix-ui/react-visually-hidden";
import { BorderBeam } from "./border-beam";
import { StarsBackground } from "./stars-background";
import { useApplicationStore } from "@/store/use-application-store";

interface DialogProps {
	className?: string;
	open: boolean | undefined;
	onOpenChange: (open: boolean) => void;
	children: React.ReactNode;
}

const Dialog: FC<DialogProps> = ({ className, open, onOpenChange, children }) => {

	const { containerRef } = useRefStore();
	const { menuDeathActive } = useApplicationStore();

	return (
		<DialogPrimitive.Root open={open} onOpenChange={onOpenChange}>
			<DialogPrimitive.Portal container={containerRef?.current}>
				<DialogPrimitive.Overlay
					className="fixed inset-0 z-50 bg-black/80 backdrop-blur-sm data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0"
				/>
				<DialogPrimitive.Content
					className={cn(
						"outline-none fixed left-[50%] top-[50%] z-50 grid w-full max-w-lg translate-x-[-50%] translate-y-[-50%] gap-4 border bg-background p-6 shadow-lg duration-200 data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 data-[state=closed]:slide-out-to-left-1/2 data-[state=closed]:slide-out-to-top-[48%] data-[state=open]:slide-in-from-left-1/2 data-[state=open]:slide-in-from-top-[48%] sm:rounded-3xl",
						className
					)}
				>
					{!menuDeathActive && <BorderBeam />}
					<StarsBackground className="absolute z-0" />

					<VisuallyHidden.Root>
						<div className="flex flex-col space-y-1.5 text-center sm:text-left">
							<DialogPrimitive.Title className="text-lg font-semibold leading-none tracking-tight">null</DialogPrimitive.Title>
							<DialogPrimitive.Description className="text-sm text-muted-foreground">null</DialogPrimitive.Description>
						</div>
					</VisuallyHidden.Root>

					{children}

				</DialogPrimitive.Content>
			</DialogPrimitive.Portal>
		</DialogPrimitive.Root>
	)
}

export default Dialog;