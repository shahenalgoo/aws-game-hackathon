import React, { useState, useEffect, useCallback } from 'react';

interface Star {
	x: number;
	y: number;
	defaultX: number;
	defaultY: number;
	color: string;
	size: string;
	attracted: boolean;
}

interface MousePosition {
	x: number;
	y: number;
}

const useMousePosition = (): MousePosition => {
	const [mousePosition, setMousePosition] = useState<MousePosition>({ x: 0, y: 0 });

	useEffect(() => {
		const handleMouseMove = (event: MouseEvent) => {
			setMousePosition({ x: event.clientX, y: event.clientY });
		};

		window.addEventListener('mousemove', handleMouseMove);
		return () => window.removeEventListener('mousemove', handleMouseMove);
	}, []);

	return mousePosition;
};

const StarField: React.FC = () => {
	// Constants
	const ATTRACT_SPEED = 0.5;
	const REPULSE_SPEED = 0.2;
	const STAR_NUMBER = 400;
	const STAR_COLOR = "white";

	// State
	const [stars, setStars] = useState<Star[]>([]);
	const mousePosition = useMousePosition();
	const [dimensions, setDimensions] = useState({
		width: window.innerWidth,
		height: window.innerHeight
	});

	// Initialize stars
	useEffect(() => {
		const createStars = () => {
			const newStars: Star[] = [];
			for (let i = 0; i < STAR_NUMBER; i++) {
				const size = Math.ceil(Math.random() * 3) + 2;
				const x = Math.ceil(Math.random() * dimensions.width);
				const y = Math.ceil(Math.random() * dimensions.height);
				newStars.push({
					x,
					y,
					defaultX: x,
					defaultY: y,
					color: STAR_COLOR,
					size: `${size}px`,
					attracted: false
				});
			}
			setStars(newStars);
		};

		createStars();

		const handleResize = () => {
			setDimensions({
				width: window.innerWidth,
				height: window.innerHeight
			});
		};

		window.addEventListener('resize', handleResize);
		return () => window.removeEventListener('resize', handleResize);
	}, [dimensions.width, dimensions.height]);

	// Movement functions
	const moveStar = useCallback((star: Star, mouse: MousePosition): Star => {
		const newStar = { ...star };
		if (!((Math.abs(mouse.x - star.x) < 5) && (Math.abs(mouse.y - star.y) < 5))) {
			if (star.x > mouse.x) {
				newStar.x -= ATTRACT_SPEED;
			} else {
				newStar.x += ATTRACT_SPEED;
			}
			if (star.y > mouse.y) {
				newStar.y -= ATTRACT_SPEED;
			} else {
				newStar.y += ATTRACT_SPEED;
			}
		}
		return newStar;
	}, []);

	const repulseStar = useCallback((star: Star): Star => {
		const newStar = { ...star };
		if (!(star.x === star.defaultX && star.y === star.defaultY)) {
			if (star.x < star.defaultX) {
				newStar.x += REPULSE_SPEED;
			} else {
				newStar.x -= REPULSE_SPEED;
			}
			if (star.y < star.defaultY) {
				newStar.y += REPULSE_SPEED;
			} else {
				newStar.y -= REPULSE_SPEED;
			}
		} else {
			newStar.attracted = false;
		}
		return newStar;
	}, []);

	// Animation frame handler
	useEffect(() => {
		let animationFrameId: number;

		const updateStars = () => {
			if (stars.length === STAR_NUMBER) {
				setStars(prevStars =>
					prevStars.map(star => {
						if (
							Math.abs(mousePosition.x - star.x) < 100 &&
							Math.abs(mousePosition.y - star.y) < 100
						) {
							const movedStar = moveStar(star, mousePosition);
							movedStar.attracted = true;
							return movedStar;
						} else if (star.attracted) {
							return repulseStar(star);
						}
						return star;
					})
				);
			}
			animationFrameId = requestAnimationFrame(updateStars);
		};

		animationFrameId = requestAnimationFrame(updateStars);
		return () => cancelAnimationFrame(animationFrameId);
	}, [mousePosition, moveStar, repulseStar, stars.length]);

	return (
		<div style={{ width: '100%', height: '100%', position: 'relative' }}>
			{stars.map((star, index) => (
				<div
					key={index}
					style={{
						position: 'absolute',
						left: star.x,
						top: star.y,
						width: star.size,
						height: star.size,
						backgroundColor: star.color,
						borderRadius: '50%',
						transform: 'translate(-50%, -50%)',
						opacity: 0.5
					}}
				/>
			))}
		</div>
	);
};

export default StarField;