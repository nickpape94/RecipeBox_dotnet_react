import React, { useEffect, useState, useMemo } from 'react';
import PropTypes from 'prop-types';
import { useDropzone } from 'react-dropzone';
import {
	thumbsContainer,
	thumb,
	thumbInner,
	img,
	baseStyle,
	activeStyle,
	acceptStyle,
	rejectStyle
} from '../layout/PhotoUploadStyles';

const PhotoPreview = ({ files, setFiles, edit = false, deleteRecipePhoto }) => {
	const removeFile = (file) => () => {
		const newFiles = [ ...files ];
		newFiles.splice(newFiles.indexOf(file), 1);
		setFiles(newFiles);
	};

	const removeFileAndDelete = (file) => () => {
		const newFiles = [ ...files ];
		newFiles.splice(newFiles.indexOf(file), 1);
		setFiles(newFiles);
		deleteRecipePhoto(file.postId, file.postPhotoId);
	};

	const removeAll = () => {
		setFiles([]);
	};

	const { getRootProps, getInputProps, isDragActive, isDragAccept, isDragReject, acceptedFiles, open } = useDropzone({
		accept: 'image/*',
		onDrop: (acceptedFiles) => {
			setFiles(
				acceptedFiles.map((file) =>
					Object.assign(file, {
						preview: URL.createObjectURL(file)
					})
				)
			);
		}
	});

	console.log(edit);

	files.length = Math.min(files.length, 6);

	const style = useMemo(
		() => ({
			...baseStyle,
			...(isDragActive ? activeStyle : {}),
			...(isDragAccept ? acceptStyle : {}),
			...(isDragReject ? rejectStyle : {})
		}),
		[ isDragActive, isDragReject ]
	);

	const thumbs = files.map((file) => (
		<div className='thumb-style' style={thumb} key={file.name}>
			<div style={thumbInner}>
				{files[0].postPhotoId !== undefined && files[0].postPhotoId !== null ? (
					<img src={file.url} style={img} />
				) : (
					<img src={file.preview} style={img} />
				)}

				<button onClick={!edit ? removeFile(file) : removeFileAndDelete(file)}>
					<i className='fas fa-trash-alt fa-2x' />
				</button>
			</div>
		</div>
	));

	useEffect(
		() => () => {
			// Make sure to revoke the data uris to avoid memory leaks
			files.forEach((file) => URL.revokeObjectURL(file.preview));
		},
		[ files ]
	);

	return (
		<div className='my-2 text-center'>
			<section className='container'>
				<div {...getRootProps({ style })}>
					<input {...getInputProps()} />
					<p>Drag 'n' drop some files here</p>
					<p>(Maximum of 6 photos)</p>
					<button className='button my-1 btn btn-primary'>Open Files</button>
				</div>
				<aside style={thumbsContainer}>{thumbs}</aside>
				{files.length > 0 && (
					<button className='btn btn-danger' onClick={removeAll}>
						Remove All
					</button>
				)}
			</section>
		</div>
	);
};

export default PhotoPreview;
