import { useEffect, useState } from "react";
import MDEditor from "@uiw/react-md-editor";
import "./App.css";
import tablemark from "tablemark";

const App = () => {
	const [messages, setMessages] = useState([]);
	const [message, setMessage] = useState("");
	const aiEndpoint = "/api/Ai";
	const rawSqlEndpoint = "/api/RawSql";
	const AiAuthor = "AIDB";
	const DbAuthor = "Database";

	const handleInputChange = (e) => {
		setMessage(e.target.value);
	};

	const handleSendMessage = async () => {
		if (message.trim() === "") {
			return;
		}

		let result = {};
		appendMessage({ author: "Me", content: message });
		if (message.startsWith("/sql ")) {
			result = await executeRawSql(message.substring(5));
			appendMessage(result);
		} else {
			result = await fetchAiAnswer(message);
			appendMessage(result);
		}

		setMessage("");
	};

	const executeRawSql = async (sql) => {
		let result = await fetch(rawSqlEndpoint, {
			body: JSON.stringify({ query: sql }),
			method: "POST",
			headers: { "Content-Type": "application/json" },
		});

		let parsedResult = await result.json();
		let parsedMarkdownTable = tablemark(parsedResult);

		return { author: DbAuthor, content: parsedMarkdownTable };
	};

	const fetchAiAnswer = async (userMessage) => {
		let aiResult = await fetch(aiEndpoint, {
			body: JSON.stringify({ message: userMessage }),
			method: "POST",
			headers: { "Content-Type": "application/json" },
		});

		let parsedResult = await aiResult.json();
		console.log(parsedResult);
		return {
			author: AiAuthor,
			content: parsedResult.aiCommand,
			id: parsedResult.id,
		};
	};

	const appendMessage = (newMessage) => {
		console.log(newMessage);
		setMessages((prevMessages) => [...prevMessages, newMessage]);
	};

	const fetchSqlExecutionResult = async (commandId) => {
		var sqlResult = await fetch(`${aiEndpoint}?queryId=${commandId}`, {
			method: "GET",
		});

		var answer = await sqlResult.json();

		let parsedMarkdownTable = tablemark(answer);
		appendMessage({ author: DbAuthor, content: parsedMarkdownTable });

		return answer;
	};

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
				<div className="flex-grow bg-element rounded-md overflow-x-hidden overflow-y-auto">
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
							className="flex-1 border rounded-l-md p-2 focus:outline-none bg-element border-element"
						/>
						<button
							onClick={handleSendMessage}
							className="bg-accent2 text-white p-2 px-4 rounded-r"
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
