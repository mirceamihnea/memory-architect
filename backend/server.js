const express = require("express");

const app = express();
const PORT = 3000;

app.get("/", (req, res) => {
  res.send("Memory Architect backend running");
});

app.listen(PORT, () => {
  console.log("Server running on port " + PORT);
});