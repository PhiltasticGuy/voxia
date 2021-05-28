from flask import Flask, flash, request, redirect, url_for, session, send_file
from speechbrain.pretrained import EncoderDecoderASR
from werkzeug.utils import secure_filename
import os

ALLOWED_EXTENSIONS = {'wav'}
DATA_FOLDER = './data/recordings/'

app = Flask(__name__)
app.config['DATA_FOLDER'] = DATA_FOLDER


def allowed_file(filename):
    return '.' in filename and \
           filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

@app.route('/', methods=['GET'])
def home():
    return '''
    <!doctype html>
    <html>
    <head>
        <title>SpeechBrain API - Tests</title>
    </head>
    <body>
        <h1>SpeechBrain API - Tests</h1>
        <p>Upload a voice recording and get the transcript from SpeechBrain.</p>
        <form action="/transcribe" method="post" enctype="multipart/form-data">
           <p>
              <label for="fileVoice">Local File (.wav): </label><br />
              <input type="file" name="fileVoice">
           </p>
           <p>
              <input type="submit" value="Upload">
           </p>
        </form>
    </body>
    </html>
    '''

@app.route('/transcribe', methods=['POST'])
def transcribeFile():
    if request.method == 'POST':
        # Ensure that the form value exists.
        if 'fileVoice' not in request.files:
            flash('No file part')
            return redirect(request.url)
        # Ensure that the file isn't NULL.
        file = request.files['fileVoice']
        if not file:
            return redirect(request.url)
        # Check if it is an allowed extension.
        if not allowed_file(file.filename):
            return redirect(request.url)

        # Make sure that the data directory exists
        if (not os.path.exists(app.config['DATA_FOLDER'])):
            os.makedirs(app.config['DATA_FOLDER'])

        # Store a temporary copy locally.
        filename = secure_filename(os.path.basename(file.filename))
        filePath = os.path.join(app.config['DATA_FOLDER'], filename)
        file.save(filePath)

        # Transcribe the file using SpeechBrain.
        model_lang = str(os.getenv('MODEL_LANG')).lower()

        print('MODEL_LANG =', model_lang)

        if (model_lang == 'en'):
            asr_model = EncoderDecoderASR.from_hparams(source="speechbrain/asr-crdnn-rnnlm-librispeech", savedir="pretrained_models/asr-crdnn-rnnlm-librispeech")
        elif (model_lang == 'fr'):
            asr_model = EncoderDecoderASR.from_hparams(source="speechbrain/asr-crdnn-commonvoice-fr", savedir="pretrained_models/asr-crdnn-commonvoice-fr")
        else:
            return '[ERROR] Unrecognized language for the transcription model.'

        words = asr_model.transcribe_file(str(filePath))
        print(words)
        return words

    return "[ERROR] HTTP verb not supported."

if __name__ == "__main__":
    # Only for debugging while developing
    #app.run(host='0.0.0.0', debug=True, port=80)
    app.run()


