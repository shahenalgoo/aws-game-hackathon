/**
 * LEVEL LOADER
 * Display a loading animation while a new level is being generated
 * 
 */

import React from 'react';

const LevelLoader: React.FC = () => {

	const gridSize = { rows: 9, cols: 20 };

	return (
		<div>
			<div className="w-max mx-auto bg-black p-4 rounded-2xl">
				{Array.from({ length: gridSize.rows }).map((_, rowIndex) => (
					<div key={rowIndex} className="flex">
						{Array.from({ length: gridSize.cols }).map((_, cellIndex) => (
							<div
								key={`${rowIndex}-${cellIndex}`}
								className="w-8 h-8 flex items-center justify-center cursor-default bg-white/20 opacity-0"
								style={{
									animation: `pulseAnimation 1.5s ease-in-out infinite`,
									animationDelay: `${Math.random() * 3}s`,
								}}
							>
								<span className="text-sm text-black">{/* Empty cell */}</span>
							</div>
						))}
					</div>
				))}
			</div>
		</div>
	);
};

export default LevelLoader;
