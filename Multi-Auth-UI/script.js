const video = document.getElementById("video");
const statusText = document.getElementById("status");

// Ask for webcam access
navigator.mediaDevices
  .getUserMedia({ video: true })
  .then((stream) => {
    video.srcObject = stream;
  })
  .catch((error) => {
    alert("Webcam access denied!");
  });

// Function to capture current frame as base64
function getBase64Image() {
  const canvas = document.createElement("canvas");
  canvas.width = video.videoWidth;
  canvas.height = video.videoHeight;

  const ctx = canvas.getContext("2d");
  ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

  return canvas.toDataURL("image/png").split(",")[1]; // Remove base64 prefix
}

// Upload Face API
function uploadFace() {
  const email = document.getElementById("email").value;
  if (!email) return alert("Please enter email");

  const base64Image = getBase64Image();

  fetch("https://localhost:7195/api/auth/upload-face", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ email, base64Image }),
  })
    .then((res) => res.text())
    .then((msg) => (statusText.innerText = msg))
    .catch((err) => (statusText.innerText = "Upload failed!"));
}

// Face Login API
function faceLogin() {
  const email = document.getElementById("email").value;
  if (!email) return alert("Please enter email");

  const base64Image = getBase64Image();

  fetch("https://localhost:7195/api/auth/face-login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ email, base64Image }),
  })
    .then((res) => res.json())
    .then((data) => {
      if (data.token) {
        statusText.innerText = "✅ Login success! JWT Token generated.";
        console.log("Token:", data.token);
      } else {
        statusText.innerText = "❌ Face not matched.";
      }
    })
    .catch((err) => (statusText.innerText = "Login failed!"));
}
