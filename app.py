from flask import Flask, send_file, render_template_string
import os
import subprocess

app = Flask(__name__)

# Derleme işlemi ve DLL yolu
BUILD_DIR = os.path.dirname(os.path.abspath(__file__))
DLL_PATH = os.path.join(BUILD_DIR, "output", "TOHE.Roles.Crewmate.Xray.dll")

HTML_TEMPLATE = """
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="UTF-8">
    <title>TOHE X-Ray Role Downloader</title>
    <style>
        body {
            background-color: #0f172a;
            color: #f8fafc;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }
        .card {
            background: #1e293b;
            padding: 3rem;
            border-radius: 1rem;
            box-shadow: 0 10px 25px rgba(0,0,0,0.5);
            text-align: center;
            border: 1px solid #334155;
        }
        h1 { color: #38bdf8; margin-bottom: 0.5rem; }
        p { color: #94a3b8; margin-bottom: 2rem; }
        .btn {
            background: #0284c7;
            color: white;
            padding: 1rem 2rem;
            font-size: 1.2rem;
            font-weight: bold;
            text-decoration: none;
            border-radius: 0.5rem;
            transition: all 0.2s ease;
            display: inline-block;
            box-shadow: 0 4px 14px rgba(2, 132, 199, 0.4);
        }
        .btn:hover {
            background: #0369a1;
            transform: translateY(-2px);
        }
    </style>
</head>
<body>
    <div class="card">
        <h1>X-Ray Role DLL</h1>
        <p>Town Of Host-Enhanced için derlenmiş hazır mod dosyası</p>
        <a href="/download" class="btn">🚀 FULL HAZIR DLL'İ İNDİR</a>
    </div>
</body>
</html>
"""

@app.route('/')
def home():
    return render_template_string(HTML_TEMPLATE)

@app.route('/download')
def download():
    # Render üzerinde otomatik derleme tetikleme
    if not os.path.exists(DLL_PATH):
        subprocess.run(["dotnet", "build", "-c", "Release", "-o", "./output"])
    
    return send_file(DLL_PATH, as_attachment=True)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=int(os.environ.get('PORT', 5000)))
