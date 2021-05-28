FROM tiangolo/uwsgi-nginx-flask:python3.8
WORKDIR /app

# Update the existing packages, install dependencies and clean-up lists.
RUN apt-get update \
    #&& apt-get install -y gnupg2 \
         #apt-utils \
    && rm -rf /var/lib/apt/lists/*

# Install the 'pip' packages and purge the cache.
RUN pip install --upgrade pip \
    && pip install speechbrain \
    && pip cache purge

# Preload the SpeechBrain models.
RUN python -c "from speechbrain.pretrained import EncoderDecoderASR; EncoderDecoderASR.from_hparams(source='speechbrain/asr-crdnn-rnnlm-librispeech', savedir='pretrained_models/asr-crdnn-rnnlm-librispeech');"
RUN python -c "from speechbrain.pretrained import EncoderDecoderASR; EncoderDecoderASR.from_hparams(source='speechbrain/asr-crdnn-commonvoice-fr', savedir='pretrained_models/asr-crdnn-commonvoice-fr');"

COPY src/asr/speechbrain/app .