import React, { Fragment, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Redirect } from 'react-router-dom';
import { connect } from 'react-redux';
import { addRecipePhotos } from '../../actions/photo';
import { getPost } from '../../actions/post';
import { getUser } from '../../actions/user';
import { Form } from 'react-bootstrap';
import Spinner from '../layout/Spinner';

const PhotosToPost = ({ getPost, post: { post }, auth: { loading, user } }) => {
	// const [state={notLoaded:true}, setState] = useState(null);
	useEffect(() => {
		if (post !== null) {
			getPost(post.postId);
		}
	}, []);

	const userIdOfPost = post && post.userId;
	const idOfLoggedInUser = user && user.id;

	if (loading) {
		return <Spinner />;
	}
	// if (userIdOfPost !== idOfLoggedInUser || post === null) {
	// 	return <Redirect to={`/posts`} />;
	// }

	return (
		<Fragment>
			<div className='container'>
				{/* <div className='display-4 text-center mb-4'>
					<h1>File Upload</h1>
					<h1>{userIdOfPost}</h1>
					<h1>{idOfLoggedInUser}</h1>
				</div> */}

				<Form>
					<div className='mb-3'>
						<Form.File id='formcheck-api-custom' custom>
							<Form.File.Input isValid />
							<Form.File.Label data-browse='Button text'>Custom file input</Form.File.Label>
							<Form.Control.Feedback type='valid'>You did it!</Form.Control.Feedback>
						</Form.File>
					</div>
					<div className='mb-3'>
						<Form.File id='formcheck-api-regular'>
							<Form.File.Label>Regular file input</Form.File.Label>
							<Form.File.Input />
						</Form.File>
					</div>
				</Form>

				{/* {userIdOfPost !== idOfLoggedInUser && <Redirect to='/posts' />} */}
			</div>
		</Fragment>
	);
};

PhotosToPost.propTypes = {
	getPost: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	auth: state.auth
});

export default connect(mapStateToProps, { addRecipePhotos, getPost })(PhotosToPost);
