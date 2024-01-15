import { useEffect, useState } from "react";
import MDEditor from "@uiw/react-md-editor";
import "./App.css";
import tablemark from "tablemark";

const App = () => {
	const [messages, setMessages] = useState([]);
	const [message, setMessage] = useState("");
	const [isFetchingData, setIsFetchingData] = useState(false);
	const [freshData, setFreshData] = useState(null);
	const aiEndpoint = "/api/Ai";
	const AiAuthor = "AIDB";
	const DbAuthor = "Database";

	const handleInputChange = (e) => {
		setMessage(e.target.value);
	};

	const handleSendMessage = async () => {
		// Check if the message is not empty before sending
		if (message.trim() !== "") {
			// Clear the input after sending
			setMessage("");

			// Update messages to show user message && start fetching loader
			setMessages([...messages, { author: "Me", content: message }]);
			setIsFetchingData(true);

			var result = await fetchAiAnswer();
			setFreshData(result);
		}
	};

	const fetchAiAnswer = async () => {
		var aiResult = await fetch(aiEndpoint, {
			body: JSON.stringify({ message: message }),
			method: "POST",
			headers: { "Content-Type": "application/json" },
		});

		var answer = await aiResult.json();

		return answer;
	};

	const fetchSqlExecutionResult = async (commandId) => {
		console.log(commandId);

		var sqlResult = await fetch(`${aiEndpoint}?queryId=${commandId}`, {
			method: "GET",
		});

		var answer = await sqlResult.json();

		console.log(answer);
		let parsedMarkdownTable = tablemark(answer);
		setMessages([
			...messages,
			{ author: DbAuthor, content: parsedMarkdownTable },
		]);

		return answer;
	};

	useEffect(() => {
		var updateData = async () => {
			// ignore if there is no fresh data
			if (!freshData) {
				setIsFetchingData(false);
				return;
			}

			setIsFetchingData(false);
			setMessages([
				...messages,
				{
					author: AiAuthor,
					content: freshData?.aiCommand,
					id: freshData?.id,
				},
			]);
			setFreshData(null);
		};

		updateData();
	}, [freshData]);

	useEffect(() => {
		var healthCheck = async () => {
			var res = await fetch("/api/");
			console.log(res);
		};

		healthCheck();
	}, []);

	return (
		<div className="w-full h-screen flex justify-center p-4">
			<div className="w-[1024px] flex flex-col h-full">
				<div className="flex-grow bg-elementLight rounded-md overflow-x-hidden overflow-y-auto">
					{messages.length > 0 ? (
						messages.map((message, index) => (
							<div key={index} className="flex p-4 w-full">
								<div className="flex flex-col w-full">
									<div className="flex w-full">
										<div className="flex flex-col w-full">
											<div className="flex">
												<p className="text-white font-bold">{message.author}</p>
											</div>
											{message.author === AiAuthor ? (
												<>
													<p className="text-white m-2">
														<MDEditor.Markdown
															source={message.content}
															style={{ whiteSpace: "pre-wrap", width: "100%" }}
														/>
													</p>
													<button
														className="bg-accent text-black w-32 p-2 rounded-md m-2 font-bold hover:scale-110 duration-150"
														onClick={() => fetchSqlExecutionResult(message.id)}
													>
														Execute
													</button>
												</>
											) : message.author === DbAuthor ? (
												<p className="text-white m-2 w-full">
													<MDEditor.Markdown
														source={message.content}
														style={{ overflowX: "scroll", width: "100%" }}
													/>
												</p>
											) : (
												<p className="text-white m-2">{message.content}</p>
											)}
										</div>
									</div>
								</div>
							</div>
						))
					) : (
						<div className="flex h-full items-center justify-center">
							<p className="text-white">No messages yet</p>
						</div>
					)}
				</div>
				<div className="flex py-2">
					<div className="flex w-full">
						<input
							type="text"
							placeholder="Type your message..."
							value={message}
							onChange={handleInputChange}
							className="flex-1 border rounded-l p-2 focus:outline-none bg-element border-element"
						/>
						<button
							onClick={handleSendMessage}
							className="bg-accent2 text-white p-2 rounded-r"
						>
							Send
						</button>
					</div>
				</div>
			</div>
		</div>
	);
};

export default App;
