/**
 * NAVIGABLE MENU
 * A menu component that can be navigated using keyboard or mouse.
 * AWS Q helped with understanding how to implement this component as well as fixing bugs.
 */

import React, { useState, useEffect, useRef, KeyboardEvent as ReactKeyboardEvent } from 'react';

export type MenuLayout = 'vertical' | 'horizontal' | 'grid';

export interface MenuItem {
	id: string;
	[key: string]: any;
}

export interface MenuProps<T extends MenuItem> {
	items: T[];
	layout?: MenuLayout;
	gridCols?: number;
	onSelect?: (item: T) => void;
	children: (props: MenuRenderProps<T>) => React.ReactNode;
	className?: string;
}

export interface MenuRenderProps<T extends MenuItem> {
	item: T;
	isSelected: boolean;
	index: number;
}

function NavigableMenu<T extends MenuItem>({
	items,
	layout = 'vertical',
	gridCols = 3,
	onSelect,
	children,
	className = ''
}: MenuProps<T>) {
	const [selectedIndex, setSelectedIndex] = useState(0);
	const containerRef = useRef<HTMLDivElement>(null);

	// Handle navigation based on the layout
	const handleNavigation = (key: string) => {
		const totalItems = items.length;

		if (layout === 'grid') {
			const currentRow = Math.floor(selectedIndex / gridCols);
			const currentCol = selectedIndex % gridCols;
			const totalRows = Math.ceil(totalItems / gridCols);

			switch (key) {
				case 'ArrowUp':
				case 'w': {
					const newIndex = selectedIndex - gridCols;
					if (newIndex >= 0) {
						setSelectedIndex(newIndex);
					} else {
						// Wrap to bottom row in the same column
						const lastRowIndex = Math.min(
							currentCol + gridCols * (totalRows - 1),
							totalItems - 1
						);
						setSelectedIndex(lastRowIndex);
					}
					break;
				}
				case 'ArrowDown':
				case 's': {
					const newIndex = selectedIndex + gridCols;
					if (newIndex < totalItems) {
						setSelectedIndex(newIndex);
					} else {
						// Wrap to top row in the same column
						setSelectedIndex(currentCol);
					}
					break;
				}
				case 'ArrowLeft':
				case 'a': {
					if (currentCol > 0) {
						setSelectedIndex(selectedIndex - 1);
					} else {
						// Move to the previous row's last column
						const prevRow = currentRow - 1;
						if (prevRow >= 0) {
							const lastColInPrevRow = Math.min(
								(prevRow + 1) * gridCols - 1,
								totalItems - 1
							);
							setSelectedIndex(lastColInPrevRow);
						} else {
							// Wrap to last row's last column
							const lastRow = totalRows - 1;
							const lastColInLastRow = Math.min(
								(lastRow + 1) * gridCols - 1,
								totalItems - 1
							);
							setSelectedIndex(lastColInLastRow);
						}
					}
					break;
				}
				case 'ArrowRight':
				case 'd': {
					if (currentCol < gridCols - 1 && selectedIndex < totalItems - 1) {
						setSelectedIndex(selectedIndex + 1);
					} else {
						// Move to the next row's first column
						const nextRow = currentRow + 1;
						if (nextRow < totalRows && nextRow * gridCols < totalItems) {
							setSelectedIndex(nextRow * gridCols);
						} else {
							// Wrap to first row's first column
							setSelectedIndex(0);
						}
					}
					break;
				}
			}
		} else {
			// Vertical or horizontal navigation
			const isVertical = layout === 'vertical';
			const prevKeys = isVertical ? ['ArrowUp', 'w'] : ['ArrowLeft', 'a'];
			const nextKeys = isVertical ? ['ArrowDown', 's'] : ['ArrowRight', 'd'];

			if (prevKeys.includes(key)) {
				setSelectedIndex((prev) => (prev > 0 ? prev - 1 : totalItems - 1));
			} else if (nextKeys.includes(key)) {
				setSelectedIndex((prev) => (prev < totalItems - 1 ? prev + 1 : 0));
			}
		}
	};

	// Define navigation keys based on the layout
	const getNavigationKeys = (): string[] => {
		switch (layout) {
			case 'grid':
				return ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'w', 's', 'a', 'd'];
			case 'vertical':
				return ['ArrowUp', 'ArrowDown', 'w', 's'];
			case 'horizontal':
				return ['ArrowLeft', 'ArrowRight', 'a', 'd'];
			default:
				return [];
		}
	};

	// Handle keydown events for navigation
	const handleKeyDown = (e: ReactKeyboardEvent<HTMLDivElement>) => {
		const navigationKeys = getNavigationKeys();

		if (navigationKeys.includes(e.key)) {
			e.preventDefault();
			handleNavigation(e.key);
		}

		if (e.key === 'Enter') {
			e.preventDefault();
			onSelect?.(items[selectedIndex]);
		}
	};

	useEffect(() => {
		const handleGlobalKeyDown = (e: KeyboardEvent) => {
			const navigationKeys = getNavigationKeys();

			// Only handle global keys if the container is not already focused
			if (navigationKeys.includes(e.key) && document.activeElement !== containerRef.current) {
				e.preventDefault();
				containerRef.current?.focus();
				handleNavigation(e.key);
			}
		};

		window.addEventListener('keydown', handleGlobalKeyDown);
		return () => window.removeEventListener('keydown', handleGlobalKeyDown);
	}, [layout, selectedIndex]);

	useEffect(() => {
		containerRef.current?.focus();
	}, []);

	const handleContainerClick = () => {
		containerRef.current?.focus();
	};

	const getLayoutClasses = () => {
		switch (layout) {
			case 'grid':
				return `grid grid-cols-${gridCols}`;
			case 'horizontal':
				return 'flex flex-row';
			case 'vertical':
			default:
				return 'flex flex-col';
		}
	};

	return (
		<div
			ref={containerRef}
			className={`focus:outline-none ${getLayoutClasses()} ${className}`}
			onKeyDown={handleKeyDown}
			tabIndex={0}
			onClick={handleContainerClick}
		>
			{items.map((item, index) => (
				<div
					key={item.id}
					onClick={() => {
						setSelectedIndex(index);
						onSelect?.(item);
					}}
					onMouseEnter={() => setSelectedIndex(index)}
					className={`menu-item ${index === selectedIndex ? 'selected' : ''}`}
					style={{
						cursor: 'pointer',
					}}
				>
					{children({
						item,
						isSelected: index === selectedIndex,
						index,
					})}
				</div>
			))}
		</div>
	);
}

export default NavigableMenu;