/**
 * LEVEL PREVIEWER
 * Previews the generated level
 * Also has the same logic from the game to resolve the grid in case of inconsistencies
 * 
 */

import React, { useState, useEffect, FC } from 'react';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger, } from "@/components/ui/tooltip";


// Grid Resolver from the game
class GridResolver {
	private grid: number[][];
	private regions: number[][];
	private rows: number;
	private cols: number;
	private reachableRegions: Set<number>;

	constructor(grid: number[][]) {
		this.grid = grid;
		this.rows = grid.length;
		this.cols = grid[0].length;
		this.regions = Array(this.rows).fill(0).map(() => Array(this.cols).fill(0));
		this.reachableRegions = new Set<number>();
	}

	private isValidPosition(row: number, col: number): boolean {
		return row >= 0 && row < this.rows && col >= 0 && col < this.cols;
	}

	private labelRegion(row: number, col: number, regionNumber: number): void {
		if (!this.isValidPosition(row, col) || this.grid[row][col] === 0 || this.regions[row][col] !== 0) {
			return;
		}

		this.regions[row][col] = regionNumber;

		const dx = [-1, 0, 1, 0];
		const dy = [0, 1, 0, -1];

		for (let i = 0; i < 4; i++) {
			const newRow = row + dx[i];
			const newCol = col + dy[i];
			this.labelRegion(newRow, newCol, regionNumber);
		}
	}

	private connectRegionToMain(
		isolatedRegion: [number, number][],
		fixedGrid: number[][]
	): void {
		const visited = new Set<string>();
		const queue: Array<[number, number, [number, number][]]> = [];

		for (const [startRow, startCol] of isolatedRegion) {
			queue.push([startRow, startCol, []]);
			visited.add(`${startRow},${startCol}`);
		}

		while (queue.length > 0) {
			const [currentRow, currentCol, currentPath] = queue.shift()!;

			if (
				fixedGrid[currentRow][currentCol] !== 0 &&
				this.reachableRegions.has(this.regions[currentRow][currentCol])
			) {
				for (const [pathRow, pathCol] of currentPath) {
					if (fixedGrid[pathRow][pathCol] === 0) {
						fixedGrid[pathRow][pathCol] = 2;
					}
				}
				return;
			}

			const dx = [-1, 0, 1, 0];
			const dy = [0, 1, 0, -1];

			for (let i = 0; i < 4; i++) {
				const newRow = currentRow + dx[i];
				const newCol = currentCol + dy[i];
				const key = `${newRow},${newCol}`;

				if (this.isValidPosition(newRow, newCol) && !visited.has(key)) {
					const newPath = [...currentPath];
					if (fixedGrid[newRow][newCol] === 0) {
						newPath.push([newRow, newCol]);
					}
					queue.push([newRow, newCol, newPath]);
					visited.add(key);
				}
			}
		}
	}

	public fixIsolatedRegions(startRow: number, startCol: number): number[][] {
		const fixedGrid = this.grid.map(row => [...row]);
		let regionCounter = 1;

		for (let i = 0; i < this.rows; i++) {
			for (let j = 0; j < this.cols; j++) {
				if (fixedGrid[i][j] !== 0 && this.regions[i][j] === 0) {
					this.labelRegion(i, j, regionCounter);
					regionCounter++;
				}
			}
		}

		if (fixedGrid[startRow][startCol] !== 0) {
			this.reachableRegions.add(this.regions[startRow][startCol]);
		}

		const isolatedRegions = new Map<number, [number, number][]>();
		for (let i = 0; i < this.rows; i++) {
			for (let j = 0; j < this.cols; j++) {
				if (fixedGrid[i][j] !== 0 && !this.reachableRegions.has(this.regions[i][j])) {
					const regionNum = this.regions[i][j];
					if (!isolatedRegions.has(regionNum)) {
						isolatedRegions.set(regionNum, []);
					}
					isolatedRegions.get(regionNum)!.push([i, j]);
				}
			}
		}

		for (const region of isolatedRegions.values()) {
			this.connectRegionToMain(region, fixedGrid);
		}

		return fixedGrid;
	}
}


interface LevelPreviewProps {
	animate?: boolean;
	initialGrid: number[][];
}

const LevelPreview: FC<LevelPreviewProps> = ({ animate = true, initialGrid }) => {


	// Local states
	const [grid, setGrid] = useState(initialGrid);
	const [visibleCells, setVisibleCells] = useState(0);


	// Resolve the grid
	useEffect(() => {
		const resolver = new GridResolver(initialGrid);
		const fixedGrid = resolver.fixIsolatedRegions(4, 0);
		setGrid(fixedGrid);
	}, []);


	// Reveal cells one by one animation
	useEffect(() => {
		if (animate && visibleCells < initialGrid.flat().length) {
			const timeout = setTimeout(() => {
				setVisibleCells((prev) => prev + 1);
			}, 10);
			return () => clearTimeout(timeout);
		} else if (!animate) {
			// If animation is disabled, make all cells visible immediately
			setVisibleCells(initialGrid.flat().length);
		}
	}, [visibleCells, animate]);


	// Get cell color based on value
	const getCellColor = (value: number): string => {
		switch (value) {
			case 0: return '';
			case 1: return 'bg-white/10';
			case 2: return 'bg-orange-200 hover:bg-orange-300';
			case 3:
			case 4:
			case 5:
			case 6:
			case 7: return 'bg-red-400 hover:bg-red-500';
			case 8: return 'bg-primary hover:bg-primary/80 !text-black';
			default: return 'bg-white';
		}
	};


	// Get cell description based on value
	const getCellDescription = (value: number): string => {
		switch (value) {
			case 2: return 'Turret';
			case 3: return 'Spike Trap';
			case 4: return 'Ground Blades';
			case 5: return 'Pitfall';
			case 6: return 'Laser';
			case 7: return 'Turning Blades';
			case 8: return 'Extraction Point';
			default: return '';
		}
	};


	// Check if the tooltip should be shown
	const shouldShowTooltip = (value: number): boolean => {
		return value >= 2;
	};

	return (
		<TooltipProvider delayDuration={0}>
			<div className="w-max mx-auto bg-white/5 p-4 rounded-2xl">
				{grid.map((row, rowIndex) => (
					<div key={rowIndex} className="flex">
						{row.map((cell, cellIndex) => {
							const cellIndexFlat = rowIndex * initialGrid[0].length + cellIndex;
							const isVisible = cellIndexFlat < visibleCells;
							return (
								<React.Fragment key={`${rowIndex}-${cellIndex}`}>
									{isVisible && shouldShowTooltip(cell) ? (
										<Tooltip>
											<TooltipTrigger asChild>
												<div
													className={`
															w-8 h-8 flex items-center justify-center cursor-default
                            								${getCellColor(cell)} transition-opacity duration-500 opacity-100
															${animate && isVisible ? 'opacity-100' : 'opacity-0'}
														`}
												>
													<span className={`text-sm ${cell === 8 ? 'text-black' : cell > 2 ? 'text-white' : 'text-black'}`}>
														{getCellDescription(cell).charAt(0)}
													</span>
												</div>
											</TooltipTrigger>
											<TooltipContent>
												<p className="font-bold">{getCellDescription(cell)}</p>
											</TooltipContent>
										</Tooltip>
									) : (
										<div
											className={`
													w-8 h-8 flex items-center justify-center
                        							${getCellColor(cell)} transition-opacity duration-500 
													${isVisible ? 'opacity-100' : 'opacity-0'
												}`}
										>
											{cell !== 0 && (
												<span className="text-sm">
													{/* {cell} */}
												</span>
											)}
										</div>
									)}
								</React.Fragment>
							);
						})}
					</div>
				))}
			</div>
		</TooltipProvider>
	);
};

export default LevelPreview;