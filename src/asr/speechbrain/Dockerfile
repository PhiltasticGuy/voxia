FROM tiangolo/uwsgi-nginx-flask:python3.8

RUN apt-get update \
    #&& apt-get install -y gnupg2 \
         #apt-utils \
    && rm -rf /var/lib/apt/lists/*
	
RUN pip install --upgrade pip \
    && pip install flask-session \
        matplotlib \
        sklearn \
        numpy \
        requests \
        pandas \
        scipy \
        pickle5 \
        speechbrain

COPY src/asr/speechbrain/app /app