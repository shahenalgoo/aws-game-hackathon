import { FC } from "react";
// import StarField from "../starsfield";
import Image from "next/image";

interface UnityLoaderProps {
	isLoaded: boolean;
	loadingProgression: number;
}

export const UnityLoader: FC<UnityLoaderProps> = ({ isLoaded, loadingProgression }) => {


	return !isLoaded ? (
		<div className="fixed z-50 top-0 left-0 w-full h-full bg-black">

			{/* <StarField /> */}

			<div className="absolute top-0 left-0 w-full h-full flex items-center justify-center">
				<div className="max-w-72 md:max-w-2xl w-full text-white">

					{/* <Image src="/logo.png" alt="logo" width={720} height={215} className="mx-auto" priority /> */}

					<div className="space-y-2">
						<div className="text-xl">
							Loading game...
						</div>

						<div className="relative">
							<div className="absolute z-10 top-1/2 -translate-y-1/2 left-0 w-full h-4 border-2 rounded-sm" />
							<div
								className="relative z-20 h-6 bg-primary rounded-sm"
								style={{ width: `${loadingProgression * 100}%` }}
							>
								<div className="px-2 text-black font-bold">{Math.round(loadingProgression * 100)}%</div>
							</div>
						</div>

						<p className="text-sm">It might take a little longer to load the first time.</p>
					</div>

				</div>
			</div>
		</div>
	) : null;
}