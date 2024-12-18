"use client";

import { FC } from "react";
import useRefStore from "@/store/useRefStore";
import { cn } from "@/lib/utils";
import * as DialogPrimitive from "@radix-ui/react-dialog";
import { X } from "lucide-react";

interface GameDialogProps {
	className?: string;
	open: boolean | undefined;
	onOpenChange: (open: boolean) => void;
}

const GameDialog: FC<GameDialogProps> = ({ className, open, onOpenChange }) => {

	const { containerRef } = useRefStore();

	return (
		<DialogPrimitive.Root open={open} onOpenChange={onOpenChange}>
			<DialogPrimitive.Portal container={containerRef?.current}>
				<DialogPrimitive.Overlay className="fixed inset-0 z-50 bg-black/80  data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0" />
				<DialogPrimitive.Content
					className={cn(
						"fixed left-[50%] top-[50%] z-50 grid w-full max-w-lg translate-x-[-50%] translate-y-[-50%] gap-4 border bg-background p-6 shadow-lg duration-200 data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 data-[state=closed]:slide-out-to-left-1/2 data-[state=closed]:slide-out-to-top-[48%] data-[state=open]:slide-in-from-left-1/2 data-[state=open]:slide-in-from-top-[48%] sm:rounded-lg",
						className
					)}
				>

					<div className="flex flex-col space-y-1.5 text-center sm:text-left">
						<DialogPrimitive.Title className="text-lg font-semibold leading-none tracking-tight">Are you absolutely sure?</DialogPrimitive.Title>
						<DialogPrimitive.Description className="text-sm text-muted-foreground">
							This action cannot be undone. This will permanently delete your account
							and remove your data from our servers.
						</DialogPrimitive.Description>
					</div>

					<DialogPrimitive.Close className="absolute right-4 top-4 rounded-sm opacity-70 ring-offset-background transition-opacity hover:opacity-100 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:pointer-events-none data-[state=open]:bg-accent data-[state=open]:text-muted-foreground">
						<X className="h-4 w-4" />
						<span className="sr-only">Close</span>
					</DialogPrimitive.Close>
				</DialogPrimitive.Content>
			</DialogPrimitive.Portal>
		</DialogPrimitive.Root>
	)
}

export default GameDialog;