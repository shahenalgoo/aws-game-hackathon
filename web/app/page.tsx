import Login from "@/components/auth/login";

// const fetchTestApi = async () => {
// 	const res = await fetch("http://localhost:3000/api/test");
// 	return await res.json();
// };

export default async function Home() {

	// const data = await fetchTestApi();
	// console.log(data);


	return (
		<div>
			<Login />
			<hr className="my-4" />

			{/* <p>{data[0].name}</p> */}
		</div>
	);
}
