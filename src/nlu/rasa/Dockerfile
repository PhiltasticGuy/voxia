FROM	rasa/rasa:latest
ENV		MODEL_FILE="error.tar.gz"
EXPOSE	5005

WORKDIR /app

# Rasa expects the 'rasa run' command to have root access to create files.
USER root

COPY src/nlu/rasa .

ENTRYPOINT rasa run --enable-api -m models/${MODEL_FILE}